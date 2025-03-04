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
    
    bme280_settings* settings = new bme280_settings();
    settings->osr_h = BME280_OVERSAMPLING_1X;
    settings->osr_p = BME280_OVERSAMPLING_1X;
    settings->osr_t = BME280_OVERSAMPLING_1X;
    settings->filter = BME280_FILTER_COEFF_OFF;
    this->_period = new uint32_t(1000);
    this->_settings = settings;
    CHECKRSLT(
        bme280_set_sensor_settings(BME280_SEL_ALL_SETTINGS, this->_settings, this->_bme), 
        "set sensor settings"
    );
}

void Bosch::reset()
{
    CHECKRSLT(bme280_soft_reset(this->_bme), "RESET");
}

void Bosch::set_mode(uint8_t mode)
{
    int8_t result = BME280_E_NULL_PTR;
    while(result != BME280_OK){
        result = bme280_set_sensor_mode(mode, this->_bme);
        CHECKRSLT(result, "Trying to change power mode..");
        Bosch::delay_us(1000, NULL);
    }

    CHECKRSLT(result, "Changed power mode!");
}

/// @brief 
/// Gets bme280 data by Forced power mode 
/// @return bme280_data pointer
IRAM_ATTR bme280_data* Bosch::getDataForcedMode()
{
    Bosch::set_mode(BME280_POWERMODE_FORCED);
    this->_data = new bme280_data();
    int8_t rslt = BME280_E_NULL_PTR;
    uint8_t idx = 0;
    while (idx < 20) {
        ESP_LOGI(BME, "Measuring...");
        rslt = bme280_get_sensor_data(BME280_ALL, this->_data, this->_bme);

        if (rslt != BME280_OK) {
            ESP_LOGE(BME, "Failed to get sensor data");
            
            Bosch::delay_us(1000, NULL);
            idx++;

            continue;
        }

        break;
    }

    ESP_LOGI(BME, "Measure done.");
    Bosch::set_mode(BME280_POWERMODE_SLEEP);

    return this->_data;
}

/// @brief 
/// Gets bme280 data by normal power mode 
/// @attention Causes device heating
/// @return bme280_data pointer
IRAM_ATTR bme280_data* Bosch::getDataNormalMode()
{
    Bosch::set_mode(BME280_POWERMODE_NORMAL);
    this->_data = new bme280_data();
    int8_t measureDelayRslt = BME280_E_NULL_PTR;
    while(measureDelayRslt != BME280_OK){
        measureDelayRslt = bme280_cal_meas_delay(this->_period, this->_settings);
        CHECKRSLT(measureDelayRslt, "Calibrate measure delay")
    }

    int8_t rslt = BME280_E_NULL_PTR;
    uint8_t idx = 0;
    uint8_t status_reg;
    while (idx < 2) {
        rslt = bme280_get_regs(BME280_REG_STATUS, &status_reg, 1, this->_bme);

        if (rslt != BME280_OK) {
            ESP_LOGE(BME, "Failed to get regs");

            continue;
        }

        if(status_reg & BME280_STATUS_MEAS_DONE)
        {
            this->_bme->delay_us(*this->_period, this->_bme);
            rslt = bme280_get_sensor_data(BME280_ALL, this->_data, this->_bme);

            if (rslt != BME280_OK) {
                ESP_LOGE(BME, "Failed to get sensor data");

                continue;
            }

            ESP_LOGI(BME, "Part of measurement completed.");

            idx++;
        }
        Bosch::delay_us(1000, NULL);

        ESP_LOGI(BME, "Sensor, next iteration");
    }

    ESP_LOGI(BME, "Measure done.");
    Bosch::set_mode(BME280_POWERMODE_SLEEP);

    return this->_data;
}
