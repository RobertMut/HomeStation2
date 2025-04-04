#include "esp32/rom/ets_sys.h"
#include "bme280.h"
#include "driver/i2c.h"
#include <esp_log.h>
#include "bosch.h"
#include "pms3003.h"
#include "mqtt.h"
#include "mqtt_client.h"
#include "wifi_manager.h"
#include <freertos/mpu_wrappers.h>
#include "nvs_flash.h"
#include "intertask.h"
#include <memory>
#include <string>
#include <stdexcept>
#include <freertos/FreeRTOS.h>
#include <freertos/task.h>
#include <esp_pm.h>
#include "main.h"

#define DATA_QUEUE_SIZE 10;
#define TOPIC "air/readings"

TaskHandle_t mqtt_handle;
TaskHandle_t pms_handle;

SemaphoreHandle_t semaphore = xSemaphoreCreateBinary();

template<typename ... Args>
static std::string string_format(const std::string& format, Args ... args)
{
    int size_s = std::snprintf(nullptr, 0, format.c_str(), args ...) + 1;
    if( size_s <= 0 ){ throw std::runtime_error("Error during formatting."); }
    auto size = static_cast<size_t>(size_s);
    std::unique_ptr<char[]> buf(new char[ size ]);
    std::snprintf(buf.get(), size, format.c_str(), args ...);
    return std::string(buf.get(), buf.get() + size - 1); 
}
bool IRAM_ATTR i2c_init(uint8_t dev_addr)
{
    xSemaphoreTake(semaphore, portMAX_DELAY);
    bool success = false;
    i2c_config_t* i2c_config = new i2c_config_t(); 
    i2c_config->mode = I2C_MODE_MASTER,
    i2c_config->sda_io_num = GPIO_NUM_21,
    i2c_config->scl_io_num = GPIO_NUM_22,
    i2c_config->sda_pullup_en = GPIO_PULLUP_DISABLE,
    i2c_config->scl_pullup_en = GPIO_PULLUP_DISABLE,
    i2c_config->master.clk_speed = 10000;

    i2c_param_config(I2C_NUM_0, i2c_config);
    i2c_driver_install(I2C_NUM_0, I2C_MODE_MASTER, 0, 0, 0);

    i2c_cmd_handle_t cmd = i2c_cmd_link_create();
    i2c_master_start(cmd);
    i2c_master_write_byte(cmd, (dev_addr << 1) | I2C_MASTER_WRITE, true);
    i2c_master_stop(cmd);
    if(i2c_master_cmd_begin(I2C_NUM_0, cmd, (1000 / portTICK_PERIOD_MS)) != ESP_OK){ 
        ESP_LOGE("I2C", "Install err - %s", "I2C MASTER BEGIN INIT SANITY CHECK");
        success = false;
    } else {
        success = true;
    }
    i2c_cmd_link_delete(cmd);

    xSemaphoreGive(semaphore);

    return success;
}

int8_t IRAM_ATTR user_i2c_write(uint8_t reg_addr, const uint8_t *reg_data, uint32_t len, void *intf_ptr)
{
    xSemaphoreTake(semaphore, portMAX_DELAY);

    int32_t iError = 0;
    uint8_t device = *(uint8_t*)intf_ptr;
    esp_err_t espRc;
    i2c_cmd_handle_t cmd = i2c_cmd_link_create();

    i2c_master_start(cmd);

    i2c_master_write_byte(cmd, (device << 1) | I2C_MASTER_WRITE, true);
    i2c_master_write_byte(cmd, reg_addr, true);
    i2c_master_write(cmd, reg_data, len, true);

    i2c_master_stop(cmd);

    espRc = i2c_master_cmd_begin(I2C_NUM_0, cmd, 1000 / portTICK_PERIOD_MS);
    if (espRc == ESP_OK) {
        iError = 0;
    } else {
        iError = -1;
    }

    i2c_cmd_link_delete(cmd);
    
    xSemaphoreGive(semaphore);

    return iError;
}

int8_t IRAM_ATTR user_i2c_read(uint8_t reg_addr, uint8_t *reg_data, uint32_t len, void *intf_ptr)
{
    xSemaphoreTake(semaphore, portMAX_DELAY);

    int32_t iError = 0;
    esp_err_t esp_err;
    uint8_t device = *(uint8_t*)intf_ptr;
    i2c_cmd_handle_t cmd_handle = i2c_cmd_link_create();
    i2c_master_start(cmd_handle);

    i2c_master_write_byte(cmd_handle, (device << 1) | I2C_MASTER_WRITE, true);
    i2c_master_write_byte(cmd_handle, reg_addr, true);

    i2c_master_start(cmd_handle);

    i2c_master_write_byte(cmd_handle, (device << 1) | I2C_MASTER_READ, true);
    if (len > 1) {
        i2c_master_read(cmd_handle, reg_data, len - 1, I2C_MASTER_ACK);
    }
    i2c_master_read_byte(cmd_handle, reg_data + len - 1, I2C_MASTER_NACK);

    i2c_master_stop(cmd_handle);
    esp_err = i2c_master_cmd_begin(I2C_NUM_0, cmd_handle, 1000 / portTICK_PERIOD_MS);
 
    if (esp_err == ESP_OK) {
        iError = 0;
    } else {
        iError = -1;
    }
    i2c_cmd_link_delete(cmd_handle);
    
    xSemaphoreGive(semaphore);

    return iError;
}

static void print_mem() {
    ESP_LOGI(
        "MEM",
        "Free Heap: %u bytes\n"
        "  MALLOC_CAP_8BIT      %7zu bytes\n"
        "  MALLOC_CAP_DMA       %7zu bytes\n"
        "  MALLOC_CAP_SPIRAM    %7zu bytes\n"
        "  MALLOC_CAP_INTERNAL  %7zu bytes\n"
        "  MALLOC_CAP_DEFAULT   %7zu bytes\n"
        "  MALLOC_CAP_IRAM_8BIT %7zu bytes\n"
        "  MALLOC_CAP_RETENTION %7zu bytes\n",
        xPortGetFreeHeapSize(),
        heap_caps_get_free_size(MALLOC_CAP_8BIT),
        heap_caps_get_free_size(MALLOC_CAP_DMA),
        heap_caps_get_free_size(MALLOC_CAP_SPIRAM),
        heap_caps_get_free_size(MALLOC_CAP_INTERNAL),
        heap_caps_get_free_size(MALLOC_CAP_DEFAULT),
        heap_caps_get_free_size(MALLOC_CAP_IRAM_8BIT),
        heap_caps_get_free_size(MALLOC_CAP_RETENTION)
    );
}

static void pms_task(void *arg){
    PMS3003* pms = new PMS3003(GPIO_NUM_17, GPIO_NUM_16, GPIO_NUM_2, GPIO_NUM_4);
    while(1){
        pm_data_t* pms_data = pms->pms_uart_read();
        
        if(!pms_data->fail){
            data_t* data = intertask::get_data();
            data->pm1_0 = pms_data->pm1_0;
            data->pm2_5 = pms_data->pm2_5;
            data->pm10 = pms_data->pm10;

            intertask::set_data(data);
            print_mem();
        }

        delete pms_data;
        pms_data = nullptr;
        vTaskDelay(pdMS_TO_TICKS(20000));
    }
}

static void mqtt_task(void *arg){
    esp_mqtt_client_config_t* mqtt_cfg = new esp_mqtt_client_config_t();
    mqtt_cfg->broker = {
        .address = {
            .uri = "mqtt://192.168.1.217:9883",
            .hostname = "192.168.1.217",
            .path = "/mqtt",
            .port = 9883
        }
    };
    mqtt_cfg->credentials = {};
    mqtt_cfg->session = {
        .last_will = {
            .topic = TOPIC,
            .qos = 0
        },
        .protocol_ver = MQTT_PROTOCOL_V_5
    };
    mqtt_cfg->network = {
        .disable_auto_reconnect = true,
    };

    mqtt* mqtt_client = new mqtt(mqtt_cfg);
    
    while(1){
        data_t* data = intertask::get_data();
        
        mqtt_client->start();
        mqtt_client->subscribe(TOPIC);
        mqtt_client->send(TOPIC, data);
        mqtt_client->stop();
        intertask::clear_data();
        vTaskDelay(pdMS_TO_TICKS(90000));
    }
}

extern "C" void app_main(void) {
    // Don't delete, or everything is going to die. Initializes nvs partition.
    nvs_flash_init();
    xSemaphoreGive(semaphore);
    esp_netif_init();
    esp_event_loop_create_default();

    
    wifi_manager* wifi = new wifi_manager("", "");
    Bosch* bosch = new Bosch(GPIO_NUM_21, GPIO_NUM_22);

    bosch->init();
    wifi->start();
    
    xTaskCreatePinnedToCore(mqtt_task, "mqtt", 1024*5, NULL, 4, &mqtt_handle, 1);
    xTaskCreatePinnedToCore(pms_task, "pms", 1024*5, NULL, 6, &pms_handle, 1);

    for(;;){
        data_t* data = intertask::get_data();
        bme280_data* sensor_data = bosch->getDataForcedMode();
        double correctedTemperature = sensor_data->temperature - 1.1; //bme280 heating problem
        
        data->temperature = correctedTemperature;
        data->humidity = sensor_data->humidity;
        data->pressure = sensor_data->pressure;

        print_mem();

        delete sensor_data;
        sensor_data = nullptr;

        vTaskDelay(pdMS_TO_TICKS(60000));
    }
}