import {ComponentFixture, TestBed} from '@angular/core/testing';
import {Component, ElementRef} from '@angular/core';
import {of} from 'rxjs';
import {GraphType} from "../graph-type";
import {ChartComponentBaseDirective} from "./chart-component-base.directive";
import {ReadingsService} from "../services/readings.service";
import {StorageService} from "../services/storage.service";
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {ReadingsStorageCache} from "../interfaces/readings";
import {getElementById} from "../../tests/utils";
import {MockElementRef} from "../../tests/mocks";
import {ReadingsData} from "../../tests/data";

@Component({
    selector: 'mock-component',
    template: ' <canvas id="chart" appChartComponentBase></canvas>',
    standalone: true
})
class MockComponent extends ChartComponentBaseDirective {
    constructor(public override readingsService: ReadingsService,
                public override storageService: StorageService,
                public override readonly el: ElementRef<any>,
                public override graph: GraphType) {
        super(readingsService, storageService, el, GraphType.AirQuality);
    }
}

describe('ChartComponent', () => {
    let component: MockComponent;
    let fixture: ComponentFixture<MockComponent>;

    let readingsService: jasmine.SpyObj<ReadingsService>;
    let storageService: jasmine.SpyObj<StorageService>;
    let mockElementRef = new MockElementRef();
    
    describe('logic', () => {
        beforeEach(() => {
            TestBed.configureTestingModule({
                imports: [ChartComponentBaseDirective, MockComponent],
                providers: [
                    {provide: ReadingsService, useValue: jasmine.createSpyObj('ReadingsService', ["getReadings"])},
                    {provide: StorageService, useValue: jasmine.createSpyObj('StorageService', ["isCacheExpired", "getCurrentReadings", "setCurrentReadings", "getLastSelectedDeviceId"])},
                    {provide: ElementRef, useValue: mockElementRef},
                    {provide: GraphType, useValue: GraphType.Temperature}
                ],
                schemas: [CUSTOM_ELEMENTS_SCHEMA]
            });

            readingsService = TestBed.inject(ReadingsService) as jasmine.SpyObj<ReadingsService>;
            storageService = TestBed.inject(StorageService) as jasmine.SpyObj<StorageService>;

            fixture = TestBed.createComponent(MockComponent);
            component = fixture.componentInstance;

        });

        it('should create', () => {
            expect(component).toBeTruthy();
        });

        it('should fetch readings from service when cache is expired', () => {
            SetResults()
            
            fixture.detectChanges();

            expect(component.readings).toEqual(ReadingsData);
            expect(readingsService.getReadings).toHaveBeenCalledTimes(1);
            expect(storageService.isCacheExpired).toHaveBeenCalledTimes(1)
            expect(storageService.getLastSelectedDeviceId).toHaveBeenCalledTimes(1)
        });

        it('should fetch readings from cache and ignore service', () => {
            storageService.isCacheExpired.and.returnValue(false);
            storageService.getCurrentReadings.and.returnValue({
                expires: new Date(2099),
                readings: ReadingsData
            } as ReadingsStorageCache);
            storageService.getLastSelectedDeviceId.and.returnValue(Number("2"));
            
            fixture.detectChanges();

            expect(component.readings).toEqual(ReadingsData);
            expect(readingsService.getReadings).toHaveBeenCalledTimes(0);
            expect(storageService.isCacheExpired).toHaveBeenCalledTimes(1)
            expect(storageService.getLastSelectedDeviceId).toHaveBeenCalledTimes(1)
        });
    });
    describe('temperature', () => {
        beforeEach(() => {
            TestBed.configureTestingModule({
                imports: [ChartComponentBaseDirective, MockComponent],
                providers: [
                    {provide: ReadingsService, useValue: jasmine.createSpyObj('ReadingsService', ["getReadings"])},
                    {
                        provide: StorageService,
                        useValue: jasmine.createSpyObj('StorageService', ["isCacheExpired", "getCurrentReadings", "setCurrentReadings", "getLastSelectedDeviceId"])
                    },
                    {provide: ElementRef, useValue: mockElementRef},
                    {provide: GraphType, useValue: GraphType.Temperature}
                ],
                schemas: [CUSTOM_ELEMENTS_SCHEMA]
            });

            readingsService = TestBed.inject(ReadingsService) as jasmine.SpyObj<ReadingsService>;
            storageService = TestBed.inject(StorageService) as jasmine.SpyObj<StorageService>;

            fixture = TestBed.createComponent(MockComponent);
            component = fixture.componentInstance;

            jasmine.clock().install();
        });

        afterEach(() => {
            jasmine.clock().uninstall()
        });

        it('creates chart', () => {
          SetResults();
          
          fixture.detectChanges();

          let element = getElementById('chart', fixture);
          
          expect(element).toBeTruthy()
        })
    });

    describe('pressure', () => {
        beforeEach(() => {
            TestBed.configureTestingModule({
                imports: [ChartComponentBaseDirective, MockComponent],
                providers: [
                    {provide: ReadingsService, useValue: jasmine.createSpyObj('ReadingsService', ["getReadings"])},
                    {
                        provide: StorageService,
                        useValue: jasmine.createSpyObj('StorageService', ["isCacheExpired", "getCurrentReadings", "setCurrentReadings", "getLastSelectedDeviceId"])
                    },
                    {provide: ElementRef, useValue: mockElementRef},
                    {provide: GraphType, useValue: GraphType.Pressure}
                ],
                schemas: [CUSTOM_ELEMENTS_SCHEMA]
            });

            readingsService = TestBed.inject(ReadingsService) as jasmine.SpyObj<ReadingsService>;
            storageService = TestBed.inject(StorageService) as jasmine.SpyObj<StorageService>;

            fixture = TestBed.createComponent(MockComponent);
            component = fixture.componentInstance;

            jasmine.clock().install();
        });

        afterEach(() => {
            jasmine.clock().uninstall()
        });

        it('creates chart', () => {
            SetResults();

            fixture.detectChanges();

            let element = getElementById('chart', fixture);

            expect(element).toBeTruthy()
        })
    });
    function SetResults()
    {
    storageService.isCacheExpired.and.returnValue(true);
    storageService.getCurrentReadings.and.returnValue(undefined);
    storageService.getLastSelectedDeviceId.and.returnValue(Number("2"));
    readingsService.getReadings.and.returnValue(of(ReadingsData));
    }
});
