#include "esp_wifi.h"
#include "freertos/event_groups.h"
#include "esp_event.h"

#ifndef HOME_STATION_WIFI_MANAGER_H
#define HOME_STATION_WIFI_MANAGER_H

#define WIFI_CONNECTED_BIT BIT0
#define WIFI_FAIL_BIT      BIT1

class wifi_manager{
    private:
        static void wifi_event_handler(void *event_handler_arg, esp_event_base_t event_base, int32_t event_id,void *event_data);
        static bool reconnect(uint8_t retries);
        wifi_config_t* wifi_config;

    public:
        static EventGroupHandle_t s_wifi_event_group;
        wifi_manager(const char* ssid, const char* pass);
        void start();
};

#endif //HOME_STATION_WIFI_MANAGER_H