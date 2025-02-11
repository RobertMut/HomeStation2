export interface Readings {
  id: number;
  readDate: Date;
  temperature: number;
  humidity: number;
  pressure: number;
  pm2_5: number;
  pm1_0: number;
  pm10: number;
}

export interface ReadingsStorageCache {
  expires: Date;
  readings: Readings[];
}

export interface ReadingsExpireCache {
  expires: Date;
}