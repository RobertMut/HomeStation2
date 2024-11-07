import {DetailLevel} from "../detail-level";
import {ReadingType} from "../reading-type";

export interface DataForm {
  readingType: ReadingType
  startDate: Date | undefined;
  endDate: Date | undefined;
  device: number | undefined;
  selectedDetail: string;
}
