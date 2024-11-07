#include "intertask.h"
#include <esp_log.h>
#include <cstddef>

#define TAG "INTERTASK"

data_t* intertask::current_data = new data_t();

data_t *intertask::get_data()
{
    if(current_data == NULL){
        current_data = new data_t();

        return current_data;
    }
    return current_data;
}

void intertask::set_data(data_t *data)
{
    current_data = data;
}

void intertask::clear_data()
{
    current_data = NULL;
}
