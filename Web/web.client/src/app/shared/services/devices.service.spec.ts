import {HttpTestingController, provideHttpClientTesting} from '@angular/common/http/testing';
import {DevicesService} from "./devices.service";
import {TestBed} from "@angular/core/testing";
import {Device} from "../interfaces/device";
import {DeviceQuery} from "../interfaces/device-query";
import {OperationType} from "../operation-type";
import {provideHttpClient} from "@angular/common/http";
import {firstValueFrom} from "rxjs";

const api = "/api/Device/";

describe('DevicesService', () => {
  let service: DevicesService;
  let httpMock: HttpTestingController;

  const dummyDevices: Device[] = [
    { id: 1, name: 'Device 1', isKnown: true },
    { id: 2, name: 'Device 2', isKnown: false }
  ];
  
  beforeEach(() => {
    jasmine.clock().install();
    TestBed.configureTestingModule({
      providers: [ provideHttpClient(), provideHttpClientTesting() ]
    });
    service = TestBed.inject(DevicesService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    jasmine.clock().uninstall();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch devices', async () => {
    const device$ = service.getDevices();
    const promise = firstValueFrom(device$);

    const req = httpMock.expectOne(api);
    expect(req.request.method).toBe('GET');
    req.flush(dummyDevices);
    
    expect(await promise).toEqual(dummyDevices);
  });

  it('should handle empty device list', () => {
    service.getDevices().subscribe(devices => {
      expect(devices.length).toBe(0);
    });

    const req = httpMock.expectOne(api);
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('should approve or revoke a device', () => {
    const deviceQuery: DeviceQuery = { id: 1, name: 'Device 1', operation: OperationType.Approve };

    service.approveRevokeDevice(deviceQuery);

    const req = httpMock.expectOne(api + 'approve');
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual({
      id: deviceQuery.id,
      name: deviceQuery.name,
      operation: 'Approve'
    });
    req.flush({});
  });

  it('should handle error on approve or revoke device', () => {
    const deviceQuery: DeviceQuery = { id: 1, name: 'Device 1', operation: OperationType.Revoke };

    spyOn(console, 'error');

    service.approveRevokeDevice(deviceQuery);

    const req = httpMock.expectOne(api + 'approve');
    req.flush('Error', { status: 500, statusText: 'Server Error' });
    
    expect(console.error).toHaveBeenCalledTimes(1);
  });
});