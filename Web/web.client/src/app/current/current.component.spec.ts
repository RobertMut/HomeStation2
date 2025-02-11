import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { CurrentComponent } from './current.component';
import { DevicesService } from '../shared/services/devices.service';
import { Device } from '../shared/interfaces/device';
import { StorageService } from '../shared/services/storage.service';
import { ReadingsService } from '../shared/services/readings.service';
import { Readings } from '../shared/interfaces/readings';
import {getElementByClass, getElementsByClass, getItemByTestSelector} from "../tests/utils";
import { NoopAnimationsModule } from "@angular/platform-browser/animations";
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from "@angular/material/select";
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { MatCardModule } from "@angular/material/card";
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { ReactiveFormsModule } from "@angular/forms";

describe('CurrentComponent', () => {
  let fixture: ComponentFixture<CurrentComponent>;
  let instance: CurrentComponent;

  let devicesServiceSpy: jasmine.SpyObj<DevicesService>;
  let storageServiceSpy: jasmine.SpyObj<StorageService>; 
  let readingsServiceSpy: jasmine.SpyObj<ReadingsService>;

  const date = new Date(1970, 1, 1, 1, 1, 1, 1);
  const mockDevices: Device[] = [
    {id: 1, name: 'Device 1', isKnown: false},
    {id: 2, name: 'Device 2', isKnown: true}
  ];
  const reading: Readings = {
    id: 1,
    humidity: 100,
    pm1_0: 2,
    pm2_5: 1,
    pm10: 0,
    pressure: 1000,
    temperature: 30,
    readDate: date
  }

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CurrentComponent],
      providers: [
        { provide: DevicesService, useValue: jasmine.createSpyObj('DevicesService', ["approveRevokeDevice", "getDevices"]) },
        { provide: StorageService, useValue: jasmine.createSpyObj('StorageService', ["getLastSelectedDeviceId", "setLastSelectedDevice"]) },
        { provide: ReadingsService, useValue: jasmine.createSpyObj('ReadingsService', ["getLatestReading"]) }
      ],
      imports: [      
        MatFormFieldModule,
        MatSelectModule,
        MatCardModule,
        HttpClientTestingModule,
        ReactiveFormsModule,
        NoopAnimationsModule 
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
    });
    
    devicesServiceSpy = TestBed.inject(DevicesService) as jasmine.SpyObj<DevicesService>;
    storageServiceSpy = TestBed.inject(StorageService) as jasmine.SpyObj<StorageService>;
    readingsServiceSpy = TestBed.inject(ReadingsService) as jasmine.SpyObj<ReadingsService>;

    fixture = TestBed.createComponent(CurrentComponent);
    instance = fixture.componentInstance;
    
    jasmine.clock().install();
  });
  
  afterEach(() => {
    jasmine.clock().uninstall();
  });
  
  it('should create', () => {
    expect(fixture).toBeTruthy();
  });
  
  it('should initialize and fetch devices',() => {
    const numb = Number(2);
    devicesServiceSpy.getDevices.and.returnValue(of(mockDevices));
    storageServiceSpy.getLastSelectedDeviceId.and.returnValue(numb);
    readingsServiceSpy.getLatestReading.and.returnValue(of(reading));
    
    fixture.detectChanges();
    jasmine.clock().tick(20001);
    fixture.detectChanges();

    let temperature = getItemByTestSelector('temperature', fixture)?.nativeElement.textContent;
    let pm25 = getItemByTestSelector('pm2.5', fixture)?.nativeElement.textContent;
    let pm1_0 = getItemByTestSelector('pm1.0', fixture)?.nativeElement.textContent;
    let pm10 = getItemByTestSelector('pm10', fixture)?.nativeElement.textContent;
    let pressure = getItemByTestSelector('pressure', fixture)?.nativeElement.textContent;
    let humidity = getItemByTestSelector('humidity', fixture)?.nativeElement.textContent;
    let readDate = getItemByTestSelector('read-date', fixture)?.nativeElement.textContent;
    
    expect(devicesServiceSpy.getDevices).toHaveBeenCalledTimes(1);
    expect(storageServiceSpy.getLastSelectedDeviceId).toHaveBeenCalledTimes(2);
    expect(readingsServiceSpy.getLatestReading).toHaveBeenCalledTimes(1);
    expect(temperature).toBe("30");
    expect(humidity).toBe("100");
    expect(pressure).toBe("10");
    expect(pm10).toBe("0");
    expect(pm25).toBe("1");
    expect(pm1_0).toBe("2");
    expect(readDate).toBe("Reading date: Sun Feb 01 1970 01:01:01 GMT+0100 (Central European Standard Time)");
  });

  it('should initialize and be empty upon empty localStorage', () => {
    devicesServiceSpy.getDevices.and.returnValue(of(mockDevices));
    storageServiceSpy.getLastSelectedDeviceId.and.returnValue(undefined);

    fixture.detectChanges();
    jasmine.clock().tick(20001);
    fixture.detectChanges();

    let cards = getItemByTestSelector('.cards', fixture)?.nativeElement
    let temperature = getItemByTestSelector('temperature', fixture)?.nativeElement.textContent;

    expect(devicesServiceSpy.getDevices).toHaveBeenCalledTimes(1);
    expect(cards).toBeUndefined();
    expect(temperature).toBeUndefined()
  });

  it('should show warning message upon no devices', () => {
    devicesServiceSpy.getDevices.and.returnValue(of([]));
    storageServiceSpy.getLastSelectedDeviceId.and.returnValue(undefined);

    fixture.detectChanges();
    jasmine.clock().tick(20001);
    fixture.detectChanges();

    let errors = getElementsByClass('exception', fixture)?.map(x => x.nativeElement)

    expect(devicesServiceSpy.getDevices).toHaveBeenCalledTimes(1);
    expect(errors![0].textContent).toEqual("sync_problemNo devices to select!Set up the device and wait for data to be reported.");
    expect(errors![1].textContent).toEqual("warning_amberNo current data to display!Ensure device is selected and reporting data.");
  });

  it('should show warning upon selected device but no data', () => {
    devicesServiceSpy.getDevices.and.returnValue(of(mockDevices));
    storageServiceSpy.getLastSelectedDeviceId.and.returnValue(2);
    readingsServiceSpy.getLatestReading.and.returnValue(of())

    fixture.detectChanges();
    jasmine.clock().tick(20001);
    fixture.detectChanges();

    let error = getElementByClass('exception', fixture)?.nativeElement

    expect(devicesServiceSpy.getDevices).toHaveBeenCalledTimes(1);
    expect(error.textContent).toEqual("warning_amberNo current data to display!Ensure device is selected and reporting data.");
  });
});
