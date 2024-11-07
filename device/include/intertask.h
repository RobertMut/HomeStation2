#ifndef HOME_STATION_INTERTASK_H
#define HOME_STATION_INTERTASK_H

#include <stdint.h>

typedef struct {
	double temperature;
	double pressure;
	double humidity;
    uint8_t pm2_5;
    uint8_t pm1_0;
    uint8_t pm10;
} data_t;

class intertask{
    private:
        static data_t* current_data;

    public:
        static data_t* get_data();
        static void set_data(data_t* data);
        static void clear_data();
};

#endif //HOME_STATION_INTERTASK_H