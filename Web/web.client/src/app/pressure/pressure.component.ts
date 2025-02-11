import { Component, ContentChildren, ElementRef } from '@angular/core';
import {DataPickerComponent} from "../shared/dynamic/data-picker/data-picker.component";
import {GraphType} from "../shared/graph-type";
import {ChartComponentBaseDirective} from "../shared/directives/chart-component-base.directive";
import {ReadingsService} from "../shared/services/readings.service";
import {StorageService} from "../shared/services/storage.service";

@ContentChildren(DataPickerComponent)
@Component({
  selector: 'app-pressure',
  templateUrl: './pressure.component.html',
  styleUrl: './pressure.component.css'
})
export class PressureComponent extends ChartComponentBaseDirective {
  protected readonly DataPickerComponent = DataPickerComponent;
    constructor(readingsService: ReadingsService, storageService: StorageService, el: ElementRef<any>) {
        super(readingsService, storageService, el, GraphType.Pressure);
    }
}
