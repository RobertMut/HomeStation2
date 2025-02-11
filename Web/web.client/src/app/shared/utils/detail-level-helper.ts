import {DetailLevel} from "../detail-level";

export function stringToDetailLevel(value: string): DetailLevel | undefined {
    if (Object.values(DetailLevel).includes(value as DetailLevel)) {
        return value as DetailLevel;
    }
    return undefined;
}