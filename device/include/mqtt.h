#ifndef HOME_STATION_MQTT_H
#define HOME_STATION_MQTT_H

#include "esp_event.h"
#include "mqtt_client.h"
#include "intertask.h"

class mqtt {
    private:
        esp_mqtt_client_config_t* client_config;
        esp_mqtt_client_handle_t client;
        esp_mqtt5_user_property_item_t user_arr[];
        static void event_handler(void *handler_args, esp_event_base_t base, int32_t event_id, void *event_data);

    public:
        mqtt(esp_mqtt_client_config_t *client_config);
        void start();
        void send(const char *topic, data_t *data);
        void subscribe(const char *topic);
        void stop();
};

#endif //HOME_STATION_MQTT_H