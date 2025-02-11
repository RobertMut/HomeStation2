import {Component, CUSTOM_ELEMENTS_SCHEMA, EventEmitter, OnInit, Output} from '@angular/core';
import {AsyncPipe, CommonModule, NgFor } from '@angular/common';
import {Device} from "../../interfaces/device";
import {FormGroup, FormsModule, Validators, ReactiveFormsModule, FormBuilder} from '@angular/forms';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {MatButtonModule} from '@angular/material/button';
import {provideNativeDateAdapter} from '@angular/material/core';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {JsonPipe} from '@angular/common';
import {DetailLevel, Details} from "../../detail-level";
import {DataForm} from "../../interfaces/data-form";
import {DevicesService} from "../../services/devices.service";
import {Observable} from 'rxjs/internal/Observable';
import {of, startWith} from 'rxjs';

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
  request = { 
    device: undefined,
    selectedDetail: DetailLevel[DetailLevel.Normal],
    startDate: new Date(Date.now()), 
    endDate: new Date(Date.now())
  } as DataForm;
  
  protected devices: Observable<Array<Device>> = of([]);

  protected details: Details[] = [
    {value: DetailLevel[DetailLevel.Detailed]},
    {value: DetailLevel[DetailLevel.Normal]},
    {value: DetailLevel[DetailLevel.Less]}
  ]
  
  dataFormGroup: FormGroup;
  constructor(private readonly service: DevicesService, private formBuilder: FormBuilder) {
    this.onSubmit = new EventEmitter();
    this.dataFormGroup = this.formBuilder.group({
      device: [this.request.device, [Validators.required]],
      start: [this.request.startDate, [Validators.required]],
      end: [this.request.endDate, [Validators.required]],
      selectedDetail: [this.request.selectedDetail, [Validators.required]]
    })
  }
  
  ngOnInit() {
    this.getDevices();
  }

  getDevices() {
    this.devices = this.service.getDevices().pipe(
      startWith([])
    );
  }

  publishValues(){
    this.onSubmit.emit(this.request);
  }
}

