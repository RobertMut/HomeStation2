import { Component, OnInit } from '@angular/core';
import {DevicesService} from "../shared/services/devices.service";
import {Device} from "../shared/interfaces/device";
import {Observable, delay, of, startWith } from 'rxjs';
import {Readings} from "../shared/interfaces/readings";
import {StorageService} from "../shared/services/storage.service";
import {ReadingsService} from "../shared/services/readings.service";

@Component({
  selector: 'app-current',
  templateUrl: './current.component.html',
  styleUrl: './current.component.css'
})
export class CurrentComponent implements OnInit {

  protected devices: Observable<Array<Device>> = of([]);
  protected device: Device = {} as Device;
  protected reading: Observable<Readings> = of({} as Readings);
  protected loaded: boolean = false;
  protected interval: any | undefined;

  constructor(private service: DevicesService,
              private storage: StorageService,
              private readingsService: ReadingsService) {
  }
  ngOnInit(): void {
    this.getDevices();
    if(this.device == undefined || this.device == {} as Device){
      this.devices.subscribe((complete) => {
        let firstDevice: Device | undefined = complete.at(0);

        if(firstDevice != undefined){
          this.device = firstDevice;
        }
      });
    }

    let storedDevice = this.storage.getLastSelectedDeviceId();
    if(storedDevice != undefined){
      this.interval = setInterval(() => {
        this.getReading({ id: storedDevice } as Device);
      }, 20000);
    }
  }

  getDevices() {
    this.devices = this.service.getDevices().pipe(
      delay(2000),
      startWith([])
    );
  }

  getReading(device: Device){
    let deviceId = this.storage.getLastSelectedDeviceId();

    if(device != undefined || device != {} as Device){
      this.getData(device.id)
    } else if(deviceId != undefined){
      this.getData(deviceId)
    }

    if(this.interval == undefined || deviceId != device.id){
      this.interval = setInterval(() => {
        this.getReading({ id: device.id } as Device);
      }, 20000);
    }

    this.loaded = true;
  }

  private getData(deviceId: number){
    this.reading = this.readingsService.getLatestReading(deviceId)
    this.storage.setLastSelectedDevice(deviceId);
  }
}
