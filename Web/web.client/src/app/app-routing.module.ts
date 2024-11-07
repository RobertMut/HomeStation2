import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {TemperatureComponent} from "./temperature/temperature.component";
import {PressureComponent} from "./pressure/pressure.component";
import {AirQualityComponent} from "./air-quality/air-quality.component";
import {CurrentComponent} from "./current/current.component";

const routes: Routes = [
  {
    path: 'current',
    component: CurrentComponent
  },
  {
    path: 'temperature-humidity',
    component: TemperatureComponent
  },
  {
    path: 'pressure',
    component: PressureComponent
  },
  {
    path: 'air-quality',
    component: AirQualityComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
