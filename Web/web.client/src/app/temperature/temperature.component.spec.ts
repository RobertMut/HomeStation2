import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TemperatureComponent } from './temperature.component';
import {AirQualityComponent} from "../air-quality/air-quality.component";
import {ReadingsService} from "../shared/services/readings.service";
import {StorageService} from "../shared/services/storage.service";
import {MockElementRef} from "../tests/mocks";
import {NoopAnimationsModule} from "@angular/platform-browser/animations";
import {CUSTOM_ELEMENTS_SCHEMA, ElementRef} from "@angular/core";
import {GraphType} from "../shared/graph-type";
import {of} from "rxjs";
import {ReadingsData} from "../tests/data";
import {getElementByClass, getElementById} from "../tests/utils";

describe('TemperatureComponent', () => {
  let component: TemperatureComponent;
  let fixture: ComponentFixture<TemperatureComponent>;

  let readingsService: jasmine.SpyObj<ReadingsService>;
  let storageService: jasmine.SpyObj<StorageService>;
  let mockElementRef = new MockElementRef();

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TemperatureComponent],
      imports: [NoopAnimationsModule],
      providers: [
        {provide: ReadingsService, useValue: jasmine.createSpyObj('ReadingsService', ["getReadings"])},
        {provide: StorageService, useValue: jasmine.createSpyObj('StorageService', ["isCacheExpired", "getCurrentReadings", "setCurrentReadings", "getLastSelectedDeviceId"])},
        {provide: ElementRef, useValue: mockElementRef},
        {provide: GraphType, useValue: GraphType.Temperature}
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    })
        .compileComponents();

    readingsService = TestBed.inject(ReadingsService) as jasmine.SpyObj<ReadingsService>;
    storageService = TestBed.inject(StorageService) as jasmine.SpyObj<StorageService>;

    fixture = TestBed.createComponent(TemperatureComponent);
    component = fixture.componentInstance;
    jasmine.clock().install();
  });

  afterEach(() => {jasmine.clock().uninstall()});

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('creates chart', () => {
    storageService.isCacheExpired.and.returnValue(true);
    storageService.getCurrentReadings.and.returnValue(undefined);
    storageService.getLastSelectedDeviceId.and.returnValue(Number("2"));
    readingsService.getReadings.and.returnValue(of(ReadingsData));

    fixture.detectChanges();

    let element = getElementById('chart', fixture);

    expect(element).toBeTruthy()
  });

  it('shows warning message', () => {
    storageService.isCacheExpired.and.returnValue(true);
    storageService.getLastSelectedDeviceId.and.returnValue(Number("2"));
    readingsService.getReadings.and.returnValue(of([]));

    fixture.detectChanges()

    let element = getElementByClass('exception', fixture);

    expect(element).toBeTruthy();
    expect(element?.nativeElement.textContent).toEqual("insightsNo temperature data!Please select device, date and data detail level to continue..");
  })
});
