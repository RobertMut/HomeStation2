import {Component, ElementRef} from '@angular/core';
import {GraphType} from "../shared/graph-type";
import {ChartComponentBaseDirective} from "../shared/directives/chart-component-base.directive";
import {ReadingsService} from "../shared/services/readings.service";
import {StorageService} from "../shared/services/storage.service";

@Component({
  selector: 'app-air-quality',
  templateUrl: './air-quality.component.html',
  styleUrl: './air-quality.component.css'
})
export class AirQualityComponent extends ChartComponentBaseDirective{
    constructor(readingsService: ReadingsService, storageService: StorageService, el: ElementRef<any>) {
        super(readingsService, storageService, el, GraphType.AirQuality);
    }
}
