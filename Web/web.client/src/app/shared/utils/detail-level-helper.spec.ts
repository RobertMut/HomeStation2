import {DetailLevel} from "../detail-level";
import {stringToDetailLevel} from "./detail-level-helper";

describe('detail-level-helper', () => {
    it('stringToDetailLevel should return expected DetailLevel', () => {
        const expected = DetailLevel.Detailed;

        const result = stringToDetailLevel("Detailed");

        expect(result).toEqual(expected);
    });

    it('stringToDetailLevel return undefined upon unknown string', () => {

        const result = stringToDetailLevel("blahblah");

        expect(result).toBeUndefined()
    });
});
