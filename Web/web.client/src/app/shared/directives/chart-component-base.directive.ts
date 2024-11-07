import { Directive, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import {Readings} from "../interfaces/readings";
import {ReadingsService} from "../services/readings.service";
import {DataForm} from "../interfaces/data-form";
import {ReadingType} from "../reading-type";
import {GraphType} from "../graph-type";
import { formatDate } from '@angular/common';
import { Chart } from 'chart.js/auto';
import {Observable, delay, of, startWith, tap } from 'rxjs';
import { By } from '@angular/platform-browser';

@Directive({
  standalone: true,
  selector: "appChartComponentBase"
})
export class ChartComponentBaseDirective implements OnInit{
  private chartElementRef: HTMLCanvasElement = {} as HTMLCanvasElement;
  private readonly root: any;

  public isReady: boolean = false;
  public readings: Readings[] = [];
  public chart: Chart | undefined;
  constructor(protected readingsService: ReadingsService, private el: ElementRef<any>) {
    this.root = this.el.nativeElement
  }

  ngOnInit(): void {
    this.chartElementRef = this.root.querySelector("#chart") as HTMLCanvasElement;
    }

  protected getReadings(event: DataForm, type: ReadingType, chartType: GraphType) {
    this.readingsService.getReadings(
      ReadingType[type],
      event.device,
      event.startDate,
      event.endDate,
      event.selectedDetail)
      .pipe(
        delay(2000),
        startWith([]),
      ).subscribe({
      next: (v) => this.readings = v,
      complete: () => this.makeChart(chartType)
    });
  }

  private makeChart(type: GraphType) {
    if(this.chart != undefined){
      this.chart.destroy();
    }

    switch (type){
      case GraphType.Pressure:
        this.makePressureChart();
        break;
      case GraphType.Temperature:
        this.makeTemperatureChart();
        break;
      case GraphType.AirQuality:
        this.makeAirQualityChart();
        break;
    }

    this.chart?.update();
  }

  private makePressureChart(){
    this.chart = new Chart(this.chartElementRef, {
      type: 'line',
      data: {
        xLabels: [],
        datasets: [
          {
            label: 'Pressure',
            data: []
          }
        ]
      }
    });

    this.chart.data.xLabels = this.readings.map(x => formatDate(x.readDate, 'yyyy-MM-dd hh-mm-ss', 'en-US'));
    this.chart.data.datasets[0].data = this.readings.map(x => x.pressure);
  }

  private makeTemperatureChart() {
    this.chart = new Chart(this.chartElementRef, {
      type: 'line',
      data: {
        xLabels: [],
        datasets: [
          {
            label: 'Temperature',
            data: []
          },
          {
            label: 'Humidity',
            data: []
          },
        ]
      }
    });

      this.chart.data.xLabels = this.readings.map(x => formatDate(x.readDate, 'yyyy-MM-dd hh-mm-ss', 'en-US'));
      this.chart.data.datasets[0].data = this.readings.map(x => x.temperature);
      this.chart.data.datasets[1].data = this.readings.map(x => x.humidity);
  }

  private makeAirQualityChart(){
    this.chart = new Chart(this.chartElementRef, {
      type: 'line',
      data: {
        xLabels: [],
        datasets: [
          {
            label: 'PM 1.0',
            data: []
          },
          {
            label: 'PM 2.5',
            data: []
          },
          {
            label: 'PM 10',
            data: []
          }
        ]
      }
    });

    this.chart.data.xLabels = this.readings.map(x => formatDate(x.readDate, 'yyyy-MM-dd hh-mm-ss', 'en-US'));
    this.chart.data.datasets[0].data = this.readings.map(x => x.pm1_0);
    this.chart.data.datasets[1].data = this.readings.map(x => x.pm2_5);
    this.chart.data.datasets[2].data = this.readings.map(x => x.pm10);
  }
}
