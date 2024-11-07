import { HttpClientModule } from '@angular/common/http';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {AsyncPipe, CommonModule, NgFor } from '@angular/common';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import {FormsModule, ReactiveFormsModule,} from '@angular/forms';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {MatTabsModule} from '@angular/material/tabs';
import {MatButtonModule} from '@angular/material/button';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatSidenavModule} from '@angular/material/sidenav';
import {MatIconModule} from '@angular/material/icon'
import {MatListModule} from '@angular/material/list';
import {MatCardModule} from '@angular/material/card';

import { TemperatureComponent } from './temperature/temperature.component';
import { PressureComponent } from './pressure/pressure.component';
import { AirQualityComponent } from './air-quality/air-quality.component';
import { DataPickerComponent } from './shared/dynamic/data-picker/data-picker.component'
import {ReadingsService} from "./shared/services/readings.service";
import { ManageComponent } from './shared/dynamic/manage/manage.component';
import {ChartComponentBaseDirective} from "./shared/directives/chart-component-base.directive";
import { CurrentComponent } from './current/current.component';

@NgModule({
  declarations: [
    AppComponent,
    TemperatureComponent,
    PressureComponent,
    AirQualityComponent,
    CurrentComponent,
  ],
  imports: [
    BrowserModule, HttpClientModule, CommonModule, FormsModule, NgFor, AsyncPipe,
    AppRoutingModule, DataPickerComponent, ManageComponent, ChartComponentBaseDirective,
    MatFormFieldModule, MatSelectModule, MatInputModule, MatTabsModule, MatButtonModule,
    MatFormFieldModule, MatDatepickerModule, MatSidenavModule, MatIconModule, MatListModule,
    MatCardModule, ReactiveFormsModule
  ],
  providers: [
    provideAnimationsAsync()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
