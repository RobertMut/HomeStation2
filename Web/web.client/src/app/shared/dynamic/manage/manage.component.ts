// noinspection ES6UnusedImports

import {CUSTOM_ELEMENTS_SCHEMA, Component, OnInit} from '@angular/core';
import { Device} from "../../interfaces/device";
import { DeviceQuery} from "../../interfaces/device-query";
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import {MatListModule} from '@angular/material/list';
import {DevicesService} from "../../services/devices.service";
import {OperationType} from "../../operation-type";
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-manage',
  templateUrl: './manage.component.html',
  styleUrl: './manage.component.css',
  imports: [MatFormFieldModule, MatSelectModule, MatInputModule, FormsModule, MatButtonModule, MatIconModule, MatListModule, CommonModule],
  standalone: true,
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class ManageComponent implements OnInit{
  protected devices: Device[] = [];
  
  constructor(private readonly service: DevicesService) {}

  ngOnInit(): void {
      this.getDevices();
    }
  private getDevices() {
    this.service.getDevices()
      .subscribe({
        next: v => this.devices = v,
        error: e => console.error(e)
      });
  }

  putApproval(device: Device) {
    this.service.approveRevokeDevice({
      id: device.id,
      name: device.name,
      operation: device.isKnown ? OperationType.Revoke : OperationType.Approve
    } as DeviceQuery)
    
    this.devices[this.devices.findIndex(x => x.id == device.id)].isKnown = !device.isKnown;
  }
}
