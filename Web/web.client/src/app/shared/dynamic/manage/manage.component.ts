import { CUSTOM_ELEMENTS_SCHEMA, Component } from '@angular/core';
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

@Component({
  selector: 'app-manage',
  templateUrl: './manage.component.html',
  styleUrl: './manage.component.css',
  imports: [MatFormFieldModule, MatSelectModule, MatInputModule, FormsModule, MatButtonModule, MatIconModule, MatListModule],
  standalone: true,
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class ManageComponent {
  protected devices: Device[] = [];
  constructor(private service: DevicesService) {
    this.getDevices();
  }
  private getDevices() {
    this.service.getDevices()
      .subscribe((result) => {
        this.devices = result;
      }, (error) => {
        console.error(error)
      });
  }

  putApproval(device: Device) {
    console.log(device)
    this.service.approveRevokeDevice({
      id: device.id,
      name: device.name,
      operation: device.isKnown ? OperationType.Revoke : OperationType.Approve
    } as DeviceQuery)

    this.getDevices();
  }
}
