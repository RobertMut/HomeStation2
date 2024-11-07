import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {Device} from "../interfaces/device";
import { Observable } from 'rxjs/internal/Observable';
import {DeviceQuery} from "../interfaces/device-query";
import {OperationType} from "../operation-type";

const api = "/api/Device/";

@Injectable({
  providedIn: 'root'
})
export class DevicesService {

  constructor(private http: HttpClient) { }

  public getDevices() : Observable<Device[]> {
    return this.http.get<Device[]>(api)
  }

  public approveRevokeDevice(device: DeviceQuery){
    const body = {
      id: device.id,
      name: device.name,
      operation: OperationType[device.operation]
    };
    this.http.put(api + 'approve', body).subscribe({
      next(_) { },
      error(msg){
        console.log(msg);
      }
    })
  }
}
