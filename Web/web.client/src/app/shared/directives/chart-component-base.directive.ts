import {Directive, ElementRef, OnInit} from '@angular/core';
import {Readings} from "../interfaces/readings";
import {ReadingsService} from "../services/readings.service";
import {DataForm} from "../interfaces/data-form";
import {GraphType} from "../graph-type";
import {formatDate} from '@angular/common';
import {Chart} from 'chart.js/auto';
import {startWith} from 'rxjs';
import {StorageService} from "../services/storage.service";
import {DetailLevel} from "../detail-level";
import {stringToDetailLevel} from '../utils/detail-level-helper';

@Directive({
    standalone: true,
    selector: "appChartComponentBase"
})
export class ChartComponentBaseDirective implements OnInit {
    private chartElementRef: HTMLCanvasElement = {} as HTMLCanvasElement;
    private readonly graphType: GraphType;
    private readonly root: any;
    public readings: Readings[] = [];
    public chart: Chart | undefined;

    constructor(protected readingsService: ReadingsService,
                protected storageService: StorageService,
                protected readonly el: ElementRef<any>,
                protected graph: GraphType) {
        this.root = this.el.nativeElement;
        this.graphType = graph;
    }

    ngOnInit(): void {
        this.chartElementRef = this.root.querySelector("#chart") as HTMLCanvasElement;
        const deviceId = this.storageService.getLastSelectedDeviceId()
        const today = new Date();

        if (this.storageService.isCacheExpired() && deviceId != undefined) {
            this.readingsService.getReadings(deviceId, today, new Date(today.getDate() - 1), DetailLevel.Less)
                .subscribe({
                    next: (v) => {
                        this.storageService.setCurrentReadings(v);
                        this.readings = v;
                    },
                    complete: () => this.makeChart(this.graphType)
                })
        } else {
            this.readings = this.storageService.getCurrentReadings()?.readings ?? [];
        }

    }

    protected getReadings(event: DataForm) {
        const detailLevel = stringToDetailLevel(event.selectedDetail);
        
        if(detailLevel == undefined){
          return;
        }
        
        this.readingsService.getReadings(
            event.device,
            event.startDate,
            event.endDate,
            detailLevel)
            .pipe(startWith([]))
            .subscribe({
            next: (v) => this.readings = v,
            complete: () => this.makeChart(this.graphType)
        });
    }

    private makeChart(type: GraphType) {
        if (this.chart != undefined) {
            this.chart.destroy();
        }

        switch (type) {
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

    private makePressureChart() {
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

    private makeAirQualityChart() {
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
