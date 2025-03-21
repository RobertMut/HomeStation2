import { HttpClientModule } from '@angular/common/http';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {AsyncPipe, CommonModule, NgFor } from '@angular/common';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

import {FormsModule, ReactiveFormsModule,} from '@angular/forms';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {MatTabsModule} from '@angular/material/tabs';
import {MatButtonModule} from '@angular/material/button';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatDrawerContainer, MatSidenavModule} from '@angular/material/sidenav';
import {MatIcon, MatIconModule} from '@angular/material/icon'
import {MatListModule} from '@angular/material/list';
import {MatCardModule} from '@angular/material/card';

import { DataPickerComponent } from './shared/dynamic/data-picker/data-picker.component'
import { ManageComponent } from './shared/dynamic/manage/manage.component';
import { CurrentComponent } from './current/current.component';
import {MatProgressSpinner} from "@angular/material/progress-spinner";
import {RouterModule} from "@angular/router";
import {ChartComponent} from "./shared/components/chart.component";

@NgModule({
  declarations: [
    AppComponent,
    CurrentComponent,
  ],
    imports: [
        BrowserModule, HttpClientModule, CommonModule, FormsModule, NgFor, AsyncPipe,
        AppRoutingModule, DataPickerComponent, ManageComponent, ChartComponent,
        MatSelectModule, MatInputModule, MatTabsModule, MatButtonModule,
        MatFormFieldModule, MatDatepickerModule, MatSidenavModule, MatIconModule, MatListModule,
        MatCardModule, ReactiveFormsModule, MatProgressSpinner, MatDrawerContainer, MatIcon, RouterModule
    ],
  providers: [
    provideAnimationsAsync()
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule {}
