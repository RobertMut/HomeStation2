import {Component, ContentChildren, ViewChild} from '@angular/core';
import {DataPickerComponent} from "../shared/dynamic/data-picker/data-picker.component";
import {ReadingType} from "../shared/reading-type";
import {GraphType} from "../shared/graph-type";
import {ChartComponentBaseDirective} from "../shared/directives/chart-component-base.directive";

@ContentChildren(DataPickerComponent)
@Component({
  selector: 'app-temperature',
  templateUrl: './temperature.component.html',
  styleUrl: './temperature.component.css'
})
export class TemperatureComponent extends ChartComponentBaseDirective{
  protected readonly GraphType = GraphType;
  protected readonly ReadingType = ReadingType;
}
