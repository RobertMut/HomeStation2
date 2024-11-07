#include "mqtt.h"
#include "mqtt_client.h"
#include <esp_log.h>
#include <algorithm>
#include "intertask.h"
#include <string>

#define TAG "MQTT"
#define ERRIFNONZERO(message, code) if(code != 0) { ESP_LOGE(TAG, message, code); }

//#define USE_PROPERTY_ARR_SIZE(user_arr)       sizeof(*user_arr)/sizeof(esp_mqtt5_user_property_item_t)

void mqtt::event_handler(void *handler_args, esp_event_base_t base, int32_t event_id, void *event_data)
{
    ESP_LOGD(TAG, "Event dispatched from event loop base=%s, event_id=%" PRIi32, base, event_id);
    esp_mqtt_event_handle_t event = (esp_mqtt_event_handle_t)event_data;
    esp_mqtt_client_handle_t client = event->client;

    ESP_LOGD(TAG, "free heap size is %" PRIu32 ", minimum %" PRIu32, esp_get_free_heap_size(), esp_get_minimum_free_heap_size());
    switch ((esp_mqtt_event_id_t)event->event_id) {
        case MQTT_EVENT_CONNECTED:
            ESP_LOGI(TAG, "MQTT_EVENT_CONNECTED");
            break;
        case MQTT_EVENT_DISCONNECTED:
            ESP_LOGI(TAG, "MQTT_EVENT_DISCONNECTED");
            break;
        case MQTT_EVENT_SUBSCRIBED:
            ESP_LOGI(TAG, "MQTT_EVENT_SUBSCRIBED");
            break;
        case MQTT_EVENT_UNSUBSCRIBED:
            ESP_LOGI(TAG, "MQTT_EVENT_UNSUBSCRIBED");
            esp_mqtt_client_disconnect(client);
            break;
        case MQTT_EVENT_PUBLISHED:
            ESP_LOGI(TAG, "MQTT_EVENT_PUBLISHED");
            break;
        case MQTT_EVENT_DATA:
            ESP_LOGI(TAG, "MQTT_EVENT_DATA");
            ESP_LOGI(TAG, "TOPIC=%.*s", event->topic_len, event->topic);
            ESP_LOGI(TAG, "DATA=%.*s", event->data_len, event->data);
            break;
        case MQTT_EVENT_ERROR:
            ESP_LOGI(TAG, "MQTT_EVENT_ERROR");
            ESP_LOGI(TAG, "MQTT5 return code is %d", event->error_handle->connect_return_code);
            if (event->error_handle->error_type == MQTT_ERROR_TYPE_TCP_TRANSPORT) {
                ERRIFNONZERO("reported from esp-tls %d", event->error_handle->esp_tls_last_esp_err);
                ERRIFNONZERO("reported from tls stack %d", event->error_handle->esp_tls_stack_err);
                ERRIFNONZERO("captured as transport's socket errno %d",  event->error_handle->esp_transport_sock_errno);
                ESP_LOGI(TAG, "Last errno string (%s)", strerror(event->error_handle->esp_transport_sock_errno));
            }
            break;
        default:
            ESP_LOGI(TAG, "Other event id:%d", event->event_id);
            break;
    }
}

mqtt::mqtt(esp_mqtt_client_config_t *client_config)
{
    this->client_config = client_config;
}

void mqtt::start()
{
    esp_mqtt_client_handle_t client = esp_mqtt_client_init(this->client_config);

    esp_mqtt_client_register_event(client, MQTT_EVENT_ANY, event_handler, NULL);
    esp_mqtt_client_start(client);
    this->client = client;
}

void mqtt::send(const char *topic, data_t* data)
{    
    // Worst case scenario char (obviously data is wrong or user is dead)
    char *buffer = (char*)calloc(140, sizeof(buffer));

    // PMS data can be null thats why we potentially can pass an empty string
    sprintf(buffer, "{\"deviceid\": 1, \"temperature\": %0.1lf, \"humidity\": %0.1lf, \"pressure\": %0.1lf, \"pm1_0\": \"%s\", \"pm2_5\": \"%s\", \"pm10\": \"%s\"}",
    data->temperature, data->humidity, data->pressure, 
    std::to_string(data->pm1_0).c_str(), std::to_string(data->pm2_5).c_str(), std::to_string(data->pm10).c_str());
    
    int msg_id = esp_mqtt_client_publish(this->client, topic, buffer, strlen(buffer), 0, 0);
    ESP_LOGI(TAG, "binary sent with msg_id=%d", msg_id);
    
    free(buffer);
}

void mqtt::subscribe(const char *topic){
    int msg_id = esp_mqtt_client_subscribe(this->client, topic, 0);
    ESP_LOGI(TAG, "subscribed msg_id=%d", msg_id);
}

void mqtt::stop()
{
    esp_mqtt_client_stop(this->client);
    esp_mqtt_client_destroy(this->client);
}
