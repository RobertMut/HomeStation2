import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {Readings} from "../interfaces/readings";
import {ReadingsService} from "../services/readings.service";
import {DataForm} from "../interfaces/data-form";
import {GraphType} from "../graph-type";
import {Chart} from 'chart.js/auto';
import {stringToDetailLevel} from '../utils/detail-level-helper';
import zoomPlugin from 'chartjs-plugin-zoom';
import {DataPickerComponent} from "../dynamic/data-picker/data-picker.component";
import {MatIcon} from "@angular/material/icon";
import {ActivatedRoute} from "@angular/router";
import moment from "moment";
import 'chartjs-adapter-moment';
Chart.register(zoomPlugin);

@Component({
    standalone: true,
    selector: "app-chart-component",
    imports: [
        DataPickerComponent,
        MatIcon
    ],
    template: `
        <div id="content">
            <div class="chart-container" [style.display]='graphPresent ? "block" : "none"'>
                <canvas testid="chart-canvas" #chart appChartComponentBase></canvas>
            </div>
            <div [hidden]="graphPresent" class="exception">
                <mat-icon class="pad-bott15">insights</mat-icon>
                <p>No data!</p>
                <p>Please select device, date and data detail level to continue..</p>
            </div>
            <app-data-picker (onSubmit)="getReadings($event)"/>
        </div>`,
    styles: `
    .chart-container {
        position: relative;
        width: 90%;
        height: 400px;
    }
    #content {
        width: 100%;
        display: flex;
        flex-flow: column;
        align-items: center;
    }
    `
})
export class ChartComponent implements OnInit {
    @ViewChild('chart') chartElement!: ElementRef<any>;
    
    graphType!: GraphType;
    graphPresent: boolean = false;
    
    private chartElementRef: HTMLCanvasElement = {} as HTMLCanvasElement;
    protected readings: Readings[] | undefined;
    private chart: Chart | undefined;
    
    constructor(public readingsService: ReadingsService,
                route: ActivatedRoute) {
        route.data.subscribe(v => this.graphType = v['graphType']);
    }

    ngOnInit(): void {}

    protected getReadings(event: DataForm) {
        this.chartElementRef = this.chartElement.nativeElement as HTMLCanvasElement;
        const detailLevel = stringToDetailLevel(event.selectedDetail);
        if(detailLevel == undefined){
          return;
        }
        
        this.readingsService.getReadings(
            this.graphType,
            event.device,
            event.startDate,
            event.endDate,
            detailLevel).subscribe({
            next: (v) => this.readings = v,
            complete: () => this.makeChart(this.graphType)
        });
    }

    private makeChart(type: GraphType) {
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
        
        this.chart!.update();
        this.graphPresent = true;
    }

    private makePressureChart() {
        const labels = this.getDateLabels();
        const pressureData = this.readings?.map(x => x.pressure) || [];
        const options = this.getOptions();

        if(!this.chart) {
            this.chart = new Chart(this.chartElementRef, {
                type: "line",
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'Pressure (hPa)',
                            data: pressureData
                        },
                    ]
                },
                options: options
            });

            return;
        }

        this.chart.data!.labels = labels;
        this.chart.data.datasets[0].data = pressureData;
    }

    private makeTemperatureChart() {
        const labels = this.getDateLabels();
        const tempData = this.readings?.map(x => x.temperature) || [];
        const humidity = this.readings?.map(x => x.humidity) || [];
        const options = this.getOptions();
        
        if(!this.chart){
            this.chart = new Chart(this.chartElementRef, {
                type: "line",
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'Temperature (°C)',
                            data: tempData
                        },
                        {
                            label: "Humidity (%)",
                            data: humidity
                        }
                    ]
                },
                options: options
            });

            return;
        }

        this.chart.data!.labels = labels;
        this.chart.data.datasets[0].data = tempData;
        this.chart.data.datasets[1].data = humidity;
    }

    private makeAirQualityChart() {
        const labels = this.getDateLabels();
        const pm25Data = this.readings?.map(x => x.pm2_5) || [];
        const pm10Data = this.readings?.map(x => x.pm10) || [];
        const pm1_0Data = this.readings?.map(x => x.pm1_0) || [];
        const options = this.getOptions();

        if(!this.chart){
            this.chart = new Chart(this.chartElementRef, {
                type: "line",
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'PM 2.5',
                            data: pm25Data
                        },
                        {
                            label: 'PM 10',
                            data: pm10Data
                        },
                        {
                            label: 'PM 1.0',
                            data: pm1_0Data
                        },
                    ]
                },
                options: options
            });

            return;
        }

        this.chart.data!.labels = labels;
        this.chart.data.datasets[0].data = pm25Data;
        this.chart.data.datasets[1].data = pm10Data;
        this.chart.data.datasets[2].data = pm1_0Data;
    }
    
    private getDateLabels(): string[] | undefined{
        return this.readings?.map(x => moment(x.readDate).format("YYYY-MM-DD H:mm"));
    }
    
    private getOptions(): any{
        return {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                zoom: {
                    zoom: {
                        wheel: {
                            enabled: true
                        },
                        pinch: {
                            enabled: true
                        },
                        mode: "xy"
                    },
                    pan:{
                        enabled: true
                    }
                }
            },
            scales: {
                x: {
                    type: "time",
                    time: {
                        format: "lll",
                        tooltipFormat: 'lll'
                    },
                    ticks: {
                        autoSkip: true,
                        maxRotation: 0,
                        minRotation: 0
                    }
                }
            }
        };
    }
}
