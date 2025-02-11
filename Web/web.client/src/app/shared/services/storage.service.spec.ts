import { TestBed } from '@angular/core/testing';

import { StorageService } from './storage.service';
import {ReadingsData} from "../../tests/data";

describe('StorageService', () => {
  let service: StorageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StorageService);
  });
  
  afterEach(() => {
    localStorage.removeItem('deviceId');
    localStorage.removeItem('cache');
  })
  

  it('should set and get the last selected device ID', () => {
    service.setLastSelectedDevice(123);
    
    expect(service.getLastSelectedDeviceId()).toBe(123);
  });

  it('should return undefined if no device ID is set', () => {
    localStorage.removeItem('deviceId');
    
    expect(service.getLastSelectedDeviceId()).toBeUndefined();
  });

  it('should set and get current readings', () => {
    service.setCurrentReadings(ReadingsData);
    
    const storedReadings = service.getCurrentReadings();
    
    expect(JSON.stringify(storedReadings?.readings)).toEqual(JSON.stringify(ReadingsData));
  });

  it('should return undefined if no readings are set', () => {
    localStorage.removeItem('cache');
    expect(service.getCurrentReadings()).toBeUndefined();
  });

  it('should return true if cache is expired', () => {
    const pastDate = new Date(new Date().getTime() - (15 * 60 * 1000 + 1));
    const expiredCache = {
      expires: pastDate,
      readings: ReadingsData
    };
    
    localStorage.setItem('cache', JSON.stringify(expiredCache));
    
    expect(service.isCacheExpired()).toBeTrue();
  });

  it('should return false if cache is not expired', () => {
    const futureDate = new Date(new Date().getTime() + 1 * 60 * 1000);
    const validCache = {
      expires: futureDate,
      readings: ReadingsData
    };
    
    localStorage.setItem('cache', JSON.stringify(validCache));
    
    expect(service.isCacheExpired()).toBeFalse();
  });
});
