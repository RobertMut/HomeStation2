import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import {Readings} from "../interfaces/readings";
import {DetailLevel} from "../detail-level";

const api = "/api/Air/";
@Injectable({
  providedIn: 'root'
})
export class ReadingsService {
  constructor(private http: HttpClient) { }

  public getReadings(device: number | undefined, startDate: Date | undefined, endDate: Date | undefined, detail: DetailLevel)
    : Observable<Readings[]> {
    return this.getResponse<any[]>(device?.toString() + "/" + startDate?.toISOString() + "/" + endDate?.toISOString() + "/" + detail);
  }

  public getLatestReading(device: number) : Observable<Readings> {
    return this.getResponse<Readings>(device.toString())
  }
  private getResponse<Type>(url: string): Observable<Type>{
    return this.http.get<Type>(api + url);
  }
}
