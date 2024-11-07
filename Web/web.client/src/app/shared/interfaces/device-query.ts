import {OperationType} from "../operation-type";

export interface DeviceQuery {
  id: number,
  name: string,
  operation: OperationType
}
