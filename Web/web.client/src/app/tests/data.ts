import {Readings} from "../shared/interfaces/readings";

export const ReadingsData: Readings[] = [
    {
        id: 1,
        readDate: new Date(Date.UTC(1970, 1, 1, 1, 1, 1, 1)),
        pressure: 1013,
        temperature: 25,
        humidity: 60,
        pm1_0: 10,
        pm2_5: 20,
        pm10: 30
    },
    {
        id: 2,
        readDate: new Date(Date.UTC(2023,10,2)),
        pressure: 1015,
        temperature: 26,
        humidity: 58,
        pm1_0: 9,
        pm2_5: 18,
        pm10: 28
    },
    {
        id: 3,
        readDate: new Date(Date.UTC(2023,10,3)),
        pressure: 1014,
        temperature: 27,
        humidity: 62,
        pm1_0: 11,
        pm2_5: 22,
        pm10: 32
    }
];