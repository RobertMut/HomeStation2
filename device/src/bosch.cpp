#include "bosch.h"
#include "driver/gpio.h"
#include <driver/i2c.h>
#include <esp_log.h>
#include "bme280.h"
#include "main.h"

#define BME "BME280"
#define CHECKRSLT(result, operation)     switch (result) {\
        case 0:\
            ESP_LOGI(BME, "Sensor ok - %s", operation);\
            break;\
        case 1:\
            ESP_LOGW(BME, "Sensor warning - %s", operation);\
            break;\
        case -1:\
            ESP_LOGE(BME, "Sensor null pointer - %s", operation);\
            break;\
        case -2:\
            ESP_LOGE(BME, "Sensor communication fail - %s", operation);\
            break;\
        case -3:\
            ESP_LOGE(BME, "Sensor invalid length - %s", operation);\
            break;\
        case -4:\
            ESP_LOGE(BME, "Sensor device not found - %s", operation);\
            break;\
        case -5:\
            ESP_LOGE(BME, "Sensor sleep mode fail - %s", operation);\
            break;\
        case -6:\
            ESP_LOGE(BME, "Sensor nvm copy fail - %s", operation);\
            break;\
    }\

#define SUCCESS 0
#define FAIL -1

void Bosch::delay_us(uint32_t period, void *intf_ptr)
{
    vTaskDelay(pdMS_TO_TICKS(period));
}

Bosch::Bosch(const gpio_num_t sda, const gpio_num_t scl)
{
    this->_dev_addr = BME280_I2C_ADDR_PRIM;
    bme280_dev* dev = new bme280_dev();
    dev->read = user_i2c_read;
    dev->write = user_i2c_write;
    dev->intf = BME280_I2C_INTF;
    dev->delay_us = delay_us;
    dev->intf_ptr = &_dev_addr;
    
    this->_bme = dev;
    this->_sda = sda;
    this->_scl = scl;
}

void Bosch::init()
{
    while(!i2c_init(this->_dev_addr)){
        ESP_LOGE(BME, "Failed to init I2C, retrying..");
    };
    CHECKRSLT(bme280_init(this->_bme), "init");
    set_mode(BME280_POWERMODE_NORMAL);
}

void Bosch::reset()
{
    CHECKRSLT(bme280_soft_reset(this->_bme), "RESET");
}

void Bosch::set_mode(uint8_t mode)
{
    bme280_settings* settings = new bme280_settings();
    settings->osr_h = BME280_OVERSAMPLING_1X;
    settings->osr_p = BME280_OVERSAMPLING_16X;
    settings->osr_t = BME280_OVERSAMPLING_2X;
    settings->filter = BME280_FILTER_COEFF_16;
    settings->standby_time = BME280_STANDBY_TIME_62_5_MS;
    this->_settings = settings;

    CHECKRSLT(
        bme280_set_sensor_settings(BME280_SEL_ALL_SETTINGS, this-> _settings, this->_bme), 
        "set sensor settings"
    );
    CHECKRSLT(
        bme280_set_sensor_mode(mode, this->_bme), "set sensor mode"
    );
}

IRAM_ATTR bme280_data* Bosch::getData()
{
    uint8_t i = 0;
    this->_data = new bme280_data();
    CHECKRSLT(bme280_cal_meas_delay(this->_period, this->_settings), "calibrate measure delay");
    uint32_t period = reinterpret_cast<uint32_t>(this->_period);
    bool done = false;

    do {
        set_mode(BME280_POWERMODE_FORCED);
        vTaskDelay(pdMS_TO_TICKS(period));
        
        uint8_t result = bme280_get_sensor_data(BME280_ALL, this->_data, this->_bme);
        
        CHECKRSLT(result, "get sensor data");
        done = result == 0;
        i++;
    } while (!done && i < 50);

    return this->_data;
}
