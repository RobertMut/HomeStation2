#include <cstdint>
#include <freertos/semphr.h>

#ifndef HOME_STATION_MAIN_H
#define HOME_STATION_MAIN_H

bool i2c_init(uint8_t dev_addr);
int8_t user_i2c_write(uint8_t reg_addr, const uint8_t *reg_data, uint32_t len, void *intf_ptr);
int8_t user_i2c_read(uint8_t reg_addr, uint8_t *reg_data, uint32_t len, void *intf_ptr);
extern SemaphoreHandle_t semaphore;

#endif //HOME_STATION_MAIN_H