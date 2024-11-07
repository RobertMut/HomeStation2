#include "bme280_defs.h"
#include "esp32/rom/ets_sys.h"
#include "driver/gpio.h"
#ifndef HOME_STATION_BOSCH_H
#define HOME_STATION_BOSCH_H

class Bosch{
    private:
        uint8_t _dev_addr;
        uint32_t* _period = 0;
        bme280_dev* _bme;
        bme280_data* _data;
        bme280_settings* _settings;
        gpio_num_t _sda;
        gpio_num_t _scl;
        static void delay_us(uint32_t period, void *intf_ptr);
    
    public:
        Bosch(const gpio_num_t sda, const gpio_num_t scl);
        void init();
        void reset();
        void set_mode(uint8_t mode);
        bme280_data* getData();
};

#endif //HOME_STATION_BOSCH_H
