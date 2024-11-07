#include <stdio.h>
#include <string.h>
#include "wifi_manager.h"
#include <esp_log.h>
#include "freertos/event_groups.h"
#include "esp_event.h"

#define TAG                         "WIFI"

EventGroupHandle_t wifi_manager::s_wifi_event_group = xEventGroupCreate();

void wifi_manager::wifi_event_handler(void *event_handler_arg, esp_event_base_t event_base, int32_t event_id, void *event_data)
{
    switch (event_id) {
        case ((int)IP_EVENT_STA_GOT_IP):
            ESP_LOGE(TAG, "WIFI GOT AN IP");
            break;
        case (WIFI_EVENT_STA_START):
            ESP_LOGE(TAG, "WIFI CONNECTING");
            break;
        case (WIFI_EVENT_STA_CONNECTED):
            ESP_LOGE(TAG, "WIFI CONNECTED");
            xEventGroupSetBits(s_wifi_event_group, WIFI_CONNECTED_BIT);
            break;
        case (WIFI_EVENT_STA_DISCONNECTED):
            ESP_LOGE(TAG, "WIFI LOST CONNECTION. RECONNECTING..");
            if(!wifi_manager::reconnect(10)) {
                ESP_LOGE(TAG, "FAILED TO RECONNECT");
                xEventGroupSetBits(s_wifi_event_group, WIFI_FAIL_BIT);
            }
            esp_restart();
            break;
        default:
            ESP_LOGE(TAG, "WIFI UNKNOWN EXCEPTION %ld", event_id);
            break;
    }
}

bool wifi_manager::reconnect(uint8_t retries)
{
    esp_err_t result = ESP_FAIL;
    uint8_t retry = 0;

    while(retry < retries){
        result = esp_wifi_connect();

        if(result == ESP_OK){
            return true;
        }

        retry++;
    }

    return result == ESP_OK;
}

wifi_manager::wifi_manager(const char *ssid, const char *pass)
{
    esp_netif_t* netif = esp_netif_create_default_wifi_sta();
    esp_netif_attach_wifi_station(netif);
    
    wifi_init_config_t wifi_init = WIFI_INIT_CONFIG_DEFAULT();
    esp_wifi_init(&wifi_init);
    esp_event_handler_register(WIFI_EVENT, ESP_EVENT_ANY_ID, wifi_event_handler, NULL);
    esp_event_handler_register(IP_EVENT, IP_EVENT_STA_GOT_IP, wifi_event_handler, NULL);


    wifi_config_t* wifi_config = new wifi_config_t();

    strcpy(((char*)(wifi_config->sta.ssid)), ssid);
    strcpy(((char*)(wifi_config->sta.password)), pass);
    this->wifi_config = wifi_config;
}

void wifi_manager::start()
{
    esp_wifi_set_config(WIFI_IF_STA, this->wifi_config);
    esp_wifi_start();
    esp_wifi_set_mode(WIFI_MODE_STA);
    esp_wifi_connect();

    EventBits_t bits = xEventGroupWaitBits(wifi_manager::s_wifi_event_group,
            WIFI_CONNECTED_BIT | WIFI_FAIL_BIT,
            pdFALSE,
            pdFALSE,
            portMAX_DELAY);

    if (bits & WIFI_CONNECTED_BIT) {
        ESP_LOGI(TAG, "connected to ap SSID:%s password:%s",
                 this->wifi_config->sta.ssid, this->wifi_config->sta.password);
    } else if (bits & WIFI_FAIL_BIT) {
        ESP_LOGI(TAG, "Failed to connect to SSID:%s, password:%s",
                 this->wifi_config->sta.ssid, this->wifi_config->sta.password);
    } else {
        ESP_LOGE(TAG, "UNEXPECTED EVENT");
    }
}
