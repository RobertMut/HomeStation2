#include "pms3003.h"
#include "driver/uart.h"
#include <esp_log.h>

#define TAG "PMS3003"

PMS3003::PMS3003(const gpio_num_t tx, const gpio_num_t rx, const gpio_num_t rts, const gpio_num_t cts){
    uart_config_t* config = new uart_config_t();
    pm_config_t* pm_config = new pm_config_t();
    config->baud_rate = 9600;
    config->data_bits = UART_DATA_8_BITS;
    config->parity = UART_PARITY_DISABLE;
    config->stop_bits = UART_STOP_BITS_1;
    config->flow_ctrl = UART_HW_FLOWCTRL_DISABLE;
    config->rx_flow_ctrl_thresh = 122;

    pm_config->port = UART_NUM_2;
    pm_config->indoor = true;
    pm_config->sensor_idx = 0;
    this->_config = pm_config;


    uart_param_config(this->_config->port, config);

    uart_set_pin(this->_config->port, tx, rx, rts, cts);

    uart_driver_install(this->_config->port, 128 * 2, 0, 0, NULL, 0);
}

pm_data_t* PMS3003::decode_data(uint8_t *data, uint8_t startByte)
{   
	pm_data_t* pm = new pm_data_t();

    pm->pm1_0 = ((data[startByte]<<8) + data[startByte+1]);
    pm->pm2_5 = ((data[startByte+2]<<8) + data[startByte+3]);
    pm->pm10 = ((data[startByte+4]<<8) + data[startByte+5]);

	return pm;
}

pm_data_t* PMS3003::pms_uart_read()
{
    uint8_t data[32];
    int length = 0;
    int retry = 0;
    pm_data_t* decoded_data;
    
    do {
        length = uart_read_bytes(this->_config->port, data, 32, 100 / portTICK_PERIOD_MS);

        if(length >= 24 && data[0]==0x42 && data[1]==0x4d){
            ESP_LOGI(TAG, "UART READ %d", length);
                decoded_data = decode_data(data, this->_config->indoor ? 4 : 10);
                decoded_data->fail = false;
                return decoded_data;
        } else {
            ESP_LOGE(TAG, "Invalid frame %d", length);
            decoded_data = new pm_data_t();
            decoded_data->fail = true;
        }

        vTaskDelay(pdMS_TO_TICKS(500));

        retry++;
    } while(decoded_data->fail || retry < 50);

    return decoded_data;
}