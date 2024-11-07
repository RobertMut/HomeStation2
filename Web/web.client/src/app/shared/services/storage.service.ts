import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {

  public setLastSelectedDevice(deviceId: number){
    localStorage.setItem('deviceId', deviceId.toString());
  }

  public getLastSelectedDeviceId() : number | undefined {
    return Number(localStorage.getItem('deviceId'));
  }
}
