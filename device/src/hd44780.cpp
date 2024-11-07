#include "hd44780.h"
#include <esp32/rom/ets_sys.h>
#include <algorithm>
#include <esp_log.h>
#include <freertos/FreeRTOS.h>
#include <freertos/task.h>
#include <freertos/semphr.h>
#include "main.h"

#define MS 1000

#define BV(x) (1 << (x))

#define DELAY_CMD_LONG  (3 * MS)
#define DELAY_CMD_SHORT (60)
#define DELAY_TOGGLE    (1)
#define DELAY_INIT      (5 * MS)

#define init_delay()   do { ets_delay_us(DELAY_INIT); } while (0)
#define short_delay()  do { ets_delay_us(DELAY_CMD_SHORT); } while (0)
#define long_delay()   do { ets_delay_us(DELAY_CMD_LONG); } while (0)
#define toggle_delay() do { ets_delay_us(DELAY_TOGGLE); } while (0)

#define CMD_CLEAR        0x01
#define CMD_RETURN_HOME  0x02
#define CMD_ENTRY_MODE   0x04
#define CMD_DISPLAY_CTRL 0x08
#define CMD_SHIFT        0x10
#define CMD_FUNC_SET     0x20
#define CMD_CGRAM_ADDR   0x40
#define CMD_DDRAM_ADDR   0x80

#define ARG_MOVE_RIGHT 0x04
#define ARG_MOVE_LEFT 0x00
#define CMD_SHIFT_LEFT  (CMD_SHIFT | CMD_DISPLAY_CTRL | ARG_MOVE_LEFT)
#define CMD_SHIFT_RIGHT (CMD_SHIFT | CMD_DISPLAY_CTRL | ARG_MOVE_RIGHT)

// CMD_ENTRY_MODE
#define ARG_EM_INCREMENT    BV(1)
#define ARG_EM_SHIFT        (1)

// CMD_DISPLAY_CTRL
#define ARG_DC_DISPLAY_ON   BV(2)
#define ARG_DC_CURSOR_ON    BV(1)
#define ARG_DC_CURSOR_BLINK (1)

// CMD_FUNC_SET
#define ARG_FS_8_BIT        BV(4)
#define ARG_FS_2_LINES      BV(3)
#define ARG_FS_FONT_5X10    BV(2)

void IRAM_ATTR HD44780::init(const HD44780_t *lcd) {
    if(!lcd->write_cb){
        gpio_config_t lcdConfig;
        lcdConfig.mode = GPIO_MODE_OUTPUT;
        lcdConfig.pull_up_en = GPIO_PULLUP_DISABLE;
        lcdConfig.intr_type = GPIO_INTR_DISABLE;
        lcdConfig.pin_bit_mask = ( 1ULL << lcd->pins.d4 |
            1ULL << lcd->pins.d5 |
            1ULL << lcd->pins.d6 |
            1ULL << lcd->pins.d7 | 
            1ULL << lcd->pins.e | 
            1ULL << lcd->pins.rs);
        
        gpio_config(&lcdConfig);
    }
    
    this->_lcd = lcd;
    //change and it won't work
    for(int i = 0; i < 3; i++){
        sendNibble((CMD_FUNC_SET | ARG_FS_8_BIT) >> 4, false);
        init_delay();
    }
    
    sendNibble(CMD_FUNC_SET >> 4, false);
    short_delay();

    writeByte(CMD_FUNC_SET
        | (lcd->lines > 1 ? ARG_FS_2_LINES : 0)
        | (lcd->font == HD44780_FONT_5X10 ? ARG_FS_FONT_5X10 : 0),
    false);
    short_delay();

    control(false, false, false);
    clear();
    writeByte(CMD_ENTRY_MODE | ARG_EM_INCREMENT, false);
    short_delay();
    control(true, false, false);
}

void HD44780::clear()
{
    writeByte(CMD_CLEAR, false);
    long_delay();
}

void HD44780::control(bool on, bool cursor, bool cursorBlink){
    writeByte(CMD_DISPLAY_CTRL
        | (on ? ARG_DC_DISPLAY_ON : 0)
        | (cursor ? ARG_DC_CURSOR_ON : 0)
        | (cursorBlink ? ARG_DC_CURSOR_BLINK : 0),
        false);

        short_delay();
}

void IRAM_ATTR HD44780::sendNibble(uint8_t byte, bool rsValue){    
    if(this->_lcd->write_cb){
        uint8_t data = (((byte >> 3) & 1) << this->_lcd->pins.d7)
                        | (((byte >> 2) & 1) << this->_lcd->pins.d6)
                        | (((byte >> 1) & 1) << this->_lcd->pins.d5)
                        | ((byte * 1) << this->_lcd->pins.d4)
                        | (rsValue ? 1 << this->_lcd->pins.rs : 0);
        this->_lcd->write_cb(this->_lcd, data | (1 << this->_lcd->pins.e));
        toggle_delay();
        this->_lcd->write_cb(this->_lcd, data);
    } else {
        gpio_set_level(this->_lcd->pins.rs, rsValue);
        toggle_delay();
        gpio_set_level(this->_lcd->pins.e, true);
        gpio_set_level(this->_lcd->pins.d7, (byte >> 3) & 1);
        gpio_set_level(this->_lcd->pins.d6, (byte >> 2) & 1);
        gpio_set_level(this->_lcd->pins.d5, (byte >> 1) & 1);
        gpio_set_level(this->_lcd->pins.d4, byte & 1);
        toggle_delay();
        gpio_set_level(this->_lcd->pins.e, false);
    }
}

void HD44780::moveCursor(uint8_t column, uint8_t row)
{
    const uint8_t lineAddresses[] = {0x00, 0x40, 0x14, 0x54};
    writeByte(CMD_DDRAM_ADDR + lineAddresses[row] + column, false);
    
    this->_x = column;
    this->_y = row;

    short_delay();
}

void HD44780::sendCharacter(uint8_t num, const uint8_t *data)
{
    uint8_t bytes = this->_lcd->font == font_t::HD44780_FONT_5X8 ? 8 : 10;
    writeByte(CMD_CGRAM_ADDR + num * bytes, false);
    short_delay();

    for(uint8_t i = 0; i < bytes; i++){
        writeByte(data[i], true);
        short_delay();
    }

    moveCursor(0,0);
}

void HD44780::write(const char * text) {
    while(*text){
        writeChar(*text++);
    }
}

void HD44780::writeChar(unsigned char c)
{
    writeByte(c, true);
    short_delay();
}

void HD44780::writeByte(uint8_t byte, bool rsValue){
    
    sendNibble(byte >> 4, rsValue);
    sendNibble(byte, rsValue);
}