
#ifndef HOME_STATION_PMS3003_H
#define HOME_STATION_PMS3003_H

#include "driver/uart.h"
#include "driver/gpio.h"

struct pm_data_t {
    uint8_t pm1_0;
    uint8_t pm2_5;
    uint8_t pm10;
    bool fail;
};

struct pm_config_t {
    bool indoor;
    uint8_t sensor_idx;
    uart_port_t port;
};


class PMS3003{
    private:
        pm_config_t* _config;
        static pm_data_t* decode_data(uint8_t* data, uint8_t startByte);

    public:
        PMS3003(const gpio_num_t tx, const gpio_num_t rx, const gpio_num_t rts, const gpio_num_t cts);
        pm_data_t* pms_uart_read();
};

#endif //HOME_STATION_PMS3003_H
