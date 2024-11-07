//
// Created by robert on 23.01.24.
//
#ifndef HOME_STATION_HD44780_H
#define HOME_STATION_HD44780_H

#include <algorithm>
#include "driver/gpio.h"

typedef struct HD44780_settings HD44780_t;
typedef esp_err_t (*HD44780_write_cb_t)(const HD44780_t *lcd, uint8_t data);

enum Pins {
    D4 = 0,
    D5 = 1,
    D6 = 2,
    D7 = 3,
    RS = 4,
    E = 5
};

enum font_t
{
    HD44780_FONT_5X8 = 0,
    HD44780_FONT_5X10
};

struct HD44780_settings {
    HD44780_write_cb_t write_cb;
    struct {
        gpio_num_t rs;
        gpio_num_t e; 
        gpio_num_t d4;
        gpio_num_t d5;
        gpio_num_t d6;
        gpio_num_t d7;
    } pins;
    uint8_t lines;
    bool backlight;
    font_t font;
};


class HD44780 {

private:
    const HD44780_t* _lcd;
    unsigned int _x;
    unsigned int _y;
    void sendNibble(uint8_t byte, bool rsValue);
    void writeByte(uint8_t byte, bool rsValue);
    void control(bool on, bool cursor, bool cursorBlink);

public:
    void init(const HD44780_t *lcd);
    void clear();
    void write(const char * text);
    void writeChar(unsigned char c);
    void moveCursor(uint8_t column, uint8_t row);
    void sendCharacter(uint8_t num, const uint8_t* data);
};


#endif //HOME_STATION_HD44780_H
