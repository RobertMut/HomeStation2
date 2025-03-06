import { Component, OnInit } from '@angular/core';
import {DevicesService} from "../shared/services/devices.service";
import {Device} from "../shared/interfaces/device";
import {Observable, delay, of, startWith, filter} from 'rxjs';
import {Readings} from "../shared/interfaces/readings";
import {StorageService} from "../shared/services/storage.service";
import {ReadingsService} from "../shared/services/readings.service";

@Component({
  selector: 'app-current',
  standalone: false,
  templateUrl: './current.component.html',
  styleUrl: './current.component.css'
})
export class CurrentComponent implements OnInit {

  protected devices: Observable<Array<Device>> = of([]);
  protected device: Device | undefined;
  protected reading: Readings | undefined;
  protected loaded: boolean = false;
  protected interval: any | undefined;

  constructor(private service: DevicesService,
              private storage: StorageService,
              private readingsService: ReadingsService) {
  }
  ngOnInit(): void {
    this.devices = this.service.getDevices();
    this.devices.subscribe({
      next: value => {
        this.getReadingForStoredDevice(value);
      },
      error: error => console.error(error),
    });
  }

  getReadingForStoredDevice(devices: Device[]){
    const stored = this.storage.getLastSelectedDevice();
    const storedDevice = devices.find(x => x.name === stored);

    if(stored && storedDevice) {
      this.getData(storedDevice.id);

      this.interval = setInterval(() => {
        this.getReading(storedDevice);
      }, 20000);

      this.loaded = true;
    }

    if(stored != storedDevice?.name){
      this.storage.clear('lastSelectedDevice');
    } else {
      this.device = storedDevice;
    }
  }
  
  getReading(device: Device | undefined){
    if(!device){
      return;
    }
    this.getData(device.id);
    
    this.device = device;
    
    if(this.device){
      this.storage.setLastSelectedDevice(this.device!.name);
    }
    
    if(!this.interval){
      this.interval = setInterval(() => {
        this.getReading(device);
      }, 20000);
    }
    
  }

  private getData(deviceId: number){
    this.readingsService.getLatestReading(deviceId)
        .subscribe({
          next: v => this.reading = v,
          complete: () => {
            this.loaded = true;
          },
          error: err =>  console.error(err)
        });
  }
}
