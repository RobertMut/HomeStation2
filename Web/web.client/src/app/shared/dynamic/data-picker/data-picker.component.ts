import {Component, CUSTOM_ELEMENTS_SCHEMA, EventEmitter, OnInit, Output} from '@angular/core';
import {AsyncPipe, CommonModule, NgFor } from '@angular/common';
import {Device} from "../../interfaces/device";
import {FormGroup, FormsModule, Validators, ReactiveFormsModule, FormBuilder} from '@angular/forms';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {MatButtonModule} from '@angular/material/button';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {DetailLevel, Details} from "../../detail-level";
import {DataForm} from "../../interfaces/data-form";
import {DevicesService} from "../../services/devices.service";
import {Observable} from 'rxjs/internal/Observable';
import {of, startWith} from 'rxjs';
import { provideMomentDateAdapter } from '@angular/material-moment-adapter';
import moment from 'moment';

@Component({
  selector: 'app-data-picker',
  templateUrl: './data-picker.component.html',
  styleUrl: './data-picker.component.css',
  imports: [MatFormFieldModule, MatSelectModule, MatInputModule, FormsModule, MatButtonModule, MatDatepickerModule,
    ReactiveFormsModule, CommonModule, NgFor, AsyncPipe],
  standalone: true,
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [provideMomentDateAdapter(undefined, {useUtc: true})],
})
export class DataPickerComponent implements OnInit{
  @Output() onSubmit: EventEmitter<DataForm> = new EventEmitter();
  request: DataForm = {
    device: undefined,
    selectedDetail: DetailLevel[DetailLevel.Normal],
    startDate: moment().toDate(),
    endDate: moment().toDate()
  };

  protected devices: Observable<Array<Device>> = of([]);

  protected details: Details[] = [
    {value: DetailLevel[DetailLevel.Detailed]},
    {value: DetailLevel[DetailLevel.Normal]},
    {value: DetailLevel[DetailLevel.Less]}
  ];

  dataFormGroup: FormGroup;
Math: any;

  constructor(private readonly service: DevicesService, private formBuilder: FormBuilder) {
    this.dataFormGroup = this.formBuilder.group({
      device: [this.request.device, [Validators.required]],
      start: [this.request.startDate, [Validators.required]],
      end: [this.request.endDate, [Validators.required]],
      selectedDetail: [this.request.selectedDetail, [Validators.required]]
    });
  }

  ngOnInit() {
    this.getDevices();
  }

  getDevices() {
    this.devices = this.service.getDevices();
  }

  publishValues(){
    this.onSubmit.emit(this.request);
  }
}