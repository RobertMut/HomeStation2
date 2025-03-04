import { Injectable } from '@angular/core';
import {Readings} from "../interfaces/readings";

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  
  private readonly key = 'lastSelectedDevice';
  
  public setLastSelectedDevice(name: string){
    localStorage.setItem(this.key, name);
  }

  public getLastSelectedDevice() : string | null {
    return localStorage.getItem(this.key);
  }
  
  public clear(key: string){
    localStorage.removeItem(key);
  }
}
