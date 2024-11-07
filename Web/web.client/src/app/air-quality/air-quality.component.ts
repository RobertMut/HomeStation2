import {Component} from '@angular/core';
import {ReadingType} from "../shared/reading-type";
import {GraphType} from "../shared/graph-type";
import {ChartComponentBaseDirective} from "../shared/directives/chart-component-base.directive";

@Component({
  selector: 'app-air-quality',
  templateUrl: './air-quality.component.html',
  styleUrl: './air-quality.component.css'
})
export class AirQualityComponent extends ChartComponentBaseDirective {
  protected readonly GraphType = GraphType;
  protected readonly ReadingType = ReadingType;
}
