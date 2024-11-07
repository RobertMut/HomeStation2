import {Component, CUSTOM_ELEMENTS_SCHEMA, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {AsyncPipe, CommonModule, NgFor } from '@angular/common';
import {Device} from "../../interfaces/device";
import {FormControl, FormGroup, FormsModule, Validators, ReactiveFormsModule} from '@angular/forms';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {MatButtonModule} from '@angular/material/button';
import {provideNativeDateAdapter} from '@angular/material/core';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {JsonPipe} from '@angular/common';

import {DetailLevel, Details} from "../../detail-level";
import {DataForm} from "../../interfaces/data-form";
import {ReadingType} from "../../reading-type";
import {DevicesService} from "../../services/devices.service";
import { Observable } from 'rxjs/internal/Observable';
import {BehaviorSubject, delay, of, startWith } from 'rxjs';

@Component({
  selector: 'app-data-picker',
  templateUrl: './data-picker.component.html',
  styleUrl: './data-picker.component.css',
  imports: [MatFormFieldModule, MatSelectModule, MatInputModule, FormsModule, MatButtonModule, MatDatepickerModule,
    FormsModule, ReactiveFormsModule, JsonPipe, CommonModule, NgFor, AsyncPipe],
  standalone: true,
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [provideNativeDateAdapter()],
})
export class DataPickerComponent implements OnInit{
  @Output() onSubmit: EventEmitter<DataForm>;

  range = new FormGroup({
    start: new FormControl<Date | null>(null),
    end: new FormControl<Date | null>(null),
  });

  numberFormControl = new FormControl('', [Validators.required]);
  protected devices: Observable<Array<Device>> = of([]);

  protected details: Details[] = [
    {value: DetailLevel[DetailLevel.Detailed]},
    {value: DetailLevel[DetailLevel.Normal]},
    {value: DetailLevel[DetailLevel.Less]}
  ]

  protected form: DataForm;

  constructor(private service: DevicesService) {
    this.onSubmit = new EventEmitter();
    this.form = {} as DataForm;
  }

  ngOnInit(): void {
    this.getDevices();
  }

  getDevices() {
    this.devices = this.service.getDevices().pipe(
      delay(2000),
      startWith([])
    );
  }

  publishValues(){
    this.onSubmit.emit(this.form);
  }
}

