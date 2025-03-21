import {ComponentFixture, TestBed} from '@angular/core/testing';
import {BehaviorSubject, of} from 'rxjs';
import {GraphType} from "../graph-type";
import {ChartComponent} from "./chart.component";
import {ReadingsService} from "../services/readings.service";
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {Readings} from "../interfaces/readings";
import {getElementByClass, getElementById, getItemByTestSelector, getManyItemsByTestSelector} from "../../tests/utils";
import {MockElementRef} from "../../tests/mocks";
import {ReadingsData} from "../../tests/data";
import {ActivatedRoute, provideRouter} from "@angular/router";
import { RouterTestingModule } from '@angular/router/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { DataPickerComponent } from '../dynamic/data-picker/data-picker.component';
import { Device } from '../interfaces/device';
import { FormBuilder } from '@angular/forms';
import { group } from '@angular/animations';
import { HttpTestingController } from '@angular/common/http/testing';


describe('ChartComponent', () => {
    let component: ChartComponent;
    let fixture: ComponentFixture<ChartComponent>;
    
    let readingsService: jasmine.SpyObj<ReadingsService> = jasmine.createSpyObj('ReadingsService', ["getReadings"]);
    let activatedRoute: ActivatedRoute;

    describe('temperature', () => {
        beforeEach(() => {
            readingsService.getReadings.and.returnValue(of(ReadingsData));

            TestBed.configureTestingModule({
                imports: [ChartComponent, MockComponent, RouterTestingModule, NoopAnimationsModule],
                providers: [
                    {provide: ReadingsService, useValue: readingsService},
                    {provide: ActivatedRoute, useValue: ({ data: of({ graphType: GraphType.Temperature }) } as any) as ActivatedRoute }
                ],
                schemas: [CUSTOM_ELEMENTS_SCHEMA]
            })
            .overrideComponent(ChartComponent, {
                remove: { imports: [DataPickerComponent]},
                add: { imports: [MockComponent]}
            }).compileComponents();

            readingsService = TestBed.inject(ReadingsService) as jasmine.SpyObj<ReadingsService>;
            activatedRoute = TestBed.inject(ActivatedRoute) as ActivatedRoute;
            
            fixture = TestBed.createComponent(ChartComponent);
            component = fixture.componentInstance;
            
            component.ngOnInit();
            fixture.detectChanges();
        });

        it('creates chart', () => {
            selectFirstElementFromPicker();

          let element = getItemByTestSelector('chart-canvas', fixture);
          
          expect(element).toBeTruthy()
        })

        it('does not create chart', () => {
            let element = getElementByClass('chart-canvas', fixture);

            expect(element).toBeFalsy()
        })

        it('contains message', () => {
            let element = getElementByClass('exception', fixture);

            expect(element?.nativeNode.textContent)
            .toEqual('insightsNo data!Please select device, date and data detail level to continue..');
        })
    });

    function selectFirstElementFromPicker(){
        let select = getItemByTestSelector('select-device', fixture)?.nativeElement;
        select.click();
        fixture.detectChanges();

        let options = getManyItemsByTestSelector('select-option', fixture);
        options![0].nativeElement.click();
        fixture.detectChanges();

        let button = getItemByTestSelector('get-data-btn', fixture)?.nativeElement
        button.click();
        fixture.detectChanges();
    }
});

const Data = [
    {
        readDate: new Date('2023-01-01T00:00:00Z'),
        temperature: 22,
        pressure: 1012,
        humidity: 45,
        pm10: 50,
        pm2_5: 15,
        pm1_0: 11

    },
    {
        readDate: new Date('2023-01-01T01:00:00Z'),
        temperature: 21,
        pressure: 1013,
        humidity: 46,
        pm10: 40,
        pm2_5: 74,
        pm1_0: 23
    },
    {
        readDate: new Date('2023-01-01T02:00:00Z'),
        temperature: 20,
        pressure: 1014,
        humidity: 47,
        pm10: 70,
        pm2_5: 22,
        pm1_0: 15
    }
] as Readings[];

class MockComponent extends DataPickerComponent{
    constructor() {
        const service = jasmine.createSpyObj('DevicesService', ['getDevices']);
        service.getDevices.and.returnValue(of([ {id: 1, name: "Dev1", isKnown: true }] as Device[]));
        const builder: FormBuilder = new FormBuilder();
        super(service, builder);
    }
}