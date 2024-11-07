import { Component, ContentChildren } from '@angular/core';
import {DataPickerComponent} from "../shared/dynamic/data-picker/data-picker.component";
import {ReadingType} from "../shared/reading-type";
import {GraphType} from "../shared/graph-type";
import {ChartComponentBaseDirective} from "../shared/directives/chart-component-base.directive";

@ContentChildren(DataPickerComponent)
@Component({
  selector: 'app-pressure',
  templateUrl: './pressure.component.html',
  styleUrl: './pressure.component.css'
})
export class PressureComponent extends ChartComponentBaseDirective {
  protected readonly DataPickerComponent = DataPickerComponent;
  protected readonly GraphType = GraphType;
  protected readonly ReadingType = ReadingType;
}
