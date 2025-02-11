import { Injectable } from '@angular/core';
import {Readings, ReadingsExpireCache, ReadingsStorageCache} from "../interfaces/readings";

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  
  public setLastSelectedDevice(deviceId: number){
    localStorage.setItem('deviceId', deviceId.toString());
  }

  public getLastSelectedDeviceId() : number | undefined {
    const deviceId = localStorage.getItem('deviceId');
    return deviceId ? Number(deviceId) : undefined;
  }
  
  public setCurrentReadings(readings: Readings[]){
    let date = new Date();
    const futureTime = date.getTime() + 900000;
    
    date.setTime(futureTime);
    
    const data : ReadingsStorageCache = {
      expires: new Date(date.toISOString()),
      readings: readings
    }
    
    localStorage.setItem('cache', JSON.stringify(data));
  }
  
  public isCacheExpired() : boolean{
    const data = localStorage.getItem('cache');
    
    if(data == null){
      return false;
    }
    
    let result = JSON.parse(data) as ReadingsExpireCache;
    
    return new Date() > new Date(result.expires);
  }
  public getCurrentReadings() : ReadingsStorageCache | undefined {
    const data = localStorage.getItem('cache');
    
    if (data == null){
      return undefined;
    }
    
    return JSON.parse(data) as ReadingsStorageCache;
  }
}
