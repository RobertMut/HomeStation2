import {HttpClientTestingModule, HttpTestingController} from '@angular/common/http/testing';
import {TestBed} from '@angular/core/testing';
import {ReadingsService} from './readings.service';
import {ReadingsData} from "../../tests/data";
import {DetailLevel} from "../detail-level";

describe('ReadingsService', () => {
  let service: ReadingsService;
  let httpMock: HttpTestingController;

  const api = "/api/Air/";
  const startDate = new Date('2023-10-02');
  const endDate = new Date('2023-10-03');

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ReadingsService]
    });
    service = TestBed.inject(ReadingsService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should fetch readings', () => {

    service.getReadings(1, startDate, endDate, DetailLevel.Detailed).subscribe(readings => {
      expect(readings).toEqual(ReadingsData);
    });

    const req = httpMock.expectOne(api + `1/${startDate.toISOString()}/${endDate.toISOString()}/${DetailLevel.Detailed}`);
    expect(req.request.method).toBe('GET');
    req.flush(ReadingsData);
  });

  it('should fetch latest reading', () => {
    service.getLatestReading(1).subscribe(reading => {
      expect(reading).toEqual(ReadingsData[0]);
    });

    const req = httpMock.expectOne(api + '1');
    expect(req.request.method).toBe('GET');
    req.flush(ReadingsData[0]);
  });

  it('should handle empty readings list', () => {
    service.getReadings(1, startDate, endDate, DetailLevel.Normal).subscribe(readings => {
      expect(readings.length).toBe(0);
    });

    const req = httpMock.expectOne(api + `1/${startDate.toISOString()}/${endDate.toISOString()}/${DetailLevel.Normal}`);
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('should handle error on fetching readings', () => {
    service.getReadings(1, startDate, endDate, DetailLevel.Less).subscribe({
          next:() => fail('should have failed with the 500 error'),
          error: (error) => {
            expect(error.status).toBe(500);
          }
    });
    
    const req = httpMock.expectOne(api + `1/${startDate.toISOString()}/${endDate.toISOString()}/Less`);
    req.flush('Error', { status: 500, statusText: 'Server Error' });
  });
});