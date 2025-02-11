import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ManageComponent} from './manage.component';
import {Device} from "../../interfaces/device";
import {DevicesService} from "../../services/devices.service";
import {of} from "rxjs";
import {getElementByClass, getManyItemsByTestSelector} from "../../../tests/utils";
import {NoopAnimationsModule} from "@angular/platform-browser/animations";

describe('ManageComponent', () => {
  let component: ManageComponent;
  let fixture: ComponentFixture<ManageComponent>;
  let devicesService: jasmine.SpyObj<DevicesService>;

  const devices: Device[] = [{id: 1, name: 'Device A', isKnown: true}, 
    {id: 2, name: 'Device B', isKnown: false}, 
    {id: 3, name: 'Device C', isKnown: true}];
  
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManageComponent, NoopAnimationsModule],
      providers:[
        {provide: DevicesService, useValue: jasmine.createSpyObj('DevicesService', ['getDevices', 'approveRevokeDevice'])}
      ]
    })
    .compileComponents();

    devicesService = TestBed.inject(DevicesService) as jasmine.SpyObj<DevicesService>;
    fixture = TestBed.createComponent(ManageComponent);

    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should get devices', () => {
    devicesService.getDevices.and.returnValue(of(devices))
    fixture.detectChanges();

    let items = getManyItemsByTestSelector('device-item', fixture)?.map(x => x.nativeElement.textContent);
    let approvalIcons = getManyItemsByTestSelector('device-icon', fixture)?.map(x => x.nativeElement.textContent);
    
    expect(devicesService.getDevices).toHaveBeenCalledTimes(1);
    
    expect(items?.length).toBe(3);
    expect(items?.length).toBe(3);
    
    expect(items).toEqual(["Device A", "Device B", "Device C"]);
    expect(approvalIcons).toEqual(["cancel", "add_circle", "cancel"]);
  });

  it('put approval should get device and change icons', () => {
    const updatedDevices: Device[] = [{id: 1, name: 'Device A', isKnown: true},
      {id: 2, name: 'Device B', isKnown: true},
      {id: 3, name: 'Device C', isKnown: true}];
    
    devicesService.getDevices.and.returnValue(of(updatedDevices));
    fixture.detectChanges();
    let button = getManyItemsByTestSelector('approve-button', fixture)?.map(x => x.nativeElement)[0];
    button.click();
    fixture.detectChanges();

    let items = getManyItemsByTestSelector('device-item', fixture)?.map(x => x.nativeElement.textContent);
    let approvalIcons = getManyItemsByTestSelector('device-icon', fixture)?.map(x => x.nativeElement.textContent);
    
    expect(devicesService.getDevices).toHaveBeenCalledTimes(1);
    expect(devicesService.approveRevokeDevice).toHaveBeenCalledTimes(1);
    
    expect(items?.length).toBe(3);
    expect(items?.length).toBe(3);
    
    expect(items).toEqual(["Device A", "Device B", "Device C"]);
    expect(approvalIcons).toEqual(["add_circle", "cancel", "cancel"]);
  });

  it('should show warning if no devices', () => {
    devicesService.getDevices.and.returnValue(of());
    fixture.detectChanges();

    let error = getElementByClass('exception', fixture)?.nativeElement.textContent;
    
    expect(error).toEqual("listNo devices to display!Configure device and wait for data")
  });
});
