import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataPickerComponent } from './data-picker.component';
import {DevicesService} from "../../services/devices.service";
import { of } from 'rxjs/internal/observable/of';
import {NoopAnimationsModule} from "@angular/platform-browser/animations";
import {By} from "@angular/platform-browser";

describe('DataPickerComponent', () => {
  let fixture: ComponentFixture<DataPickerComponent>;
  let devicesService: DevicesService;
  let instance : DataPickerComponent;

  beforeEach(async () => {
    const devicesServiceSpy = jasmine.createSpyObj('DevicesService', ['getDevices']);
    devicesServiceSpy.getDevices.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [DataPickerComponent, NoopAnimationsModule],
      providers: [
        { provide: DevicesService, useValue: devicesServiceSpy },
      ],
    }).compileComponents();
    
    fixture = TestBed.createComponent(DataPickerComponent);
    fixture.detectChanges();

    devicesService = TestBed.inject(DevicesService);
    instance = fixture.componentInstance;
  });

  it('should create', () => {
    expect(fixture).toBeTruthy();
  });

  it('should emit onSubmit event when publishValues is called', () => {
    spyOn(instance.onSubmit, 'emit');
    instance.publishValues();
    expect(instance.onSubmit.emit).toHaveBeenCalledTimes(1)
  });

  it('should call getDevices when ngOnInit is called', () => {
    expect(devicesService.getDevices).toHaveBeenCalledTimes(1);
  });
});
