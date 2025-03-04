import { TestBed } from '@angular/core/testing';
import { StorageService } from './storage.service';

describe('StorageService', () => {
  let service: StorageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StorageService);
  });
  
  afterEach(() => {
    localStorage.removeItem('lastSelectedDevice');
  })
  

  it('should set and get the last selected device', () => {
    service.setLastSelectedDevice("Dev 1");
    
    expect(service.getLastSelectedDevice()).toBe("Dev 1");
  });

  it('should return undefined if no device is set', () => {
    localStorage.removeItem('lastSelectedDevice');
    
    expect(service.getLastSelectedDevice()).toBeNull();
  });
});
