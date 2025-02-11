import {By} from "@angular/platform-browser";
import {DebugElement} from "@angular/core";
import {ComponentFixture} from "@angular/core/testing";

const TestSelector = <T>(testId: string) => `[testid="${testId}"]`;
export const getItemByTestSelector = 
    <T>(testId: string, fixture: ComponentFixture<any>): DebugElement | null =>
        fixture.debugElement.query(By.css(TestSelector(testId)));

export const getManyItemsByTestSelector =
    <T>(testId: string, fixture: ComponentFixture<any>): DebugElement[] | null =>
        fixture.debugElement.queryAll(By.css(TestSelector(testId)));

export const getElementById =
    <T>(id: string, fixture: ComponentFixture<any>): DebugElement | null =>
        fixture.debugElement.query(By.css(`#${id}`));

export const getElementByClass =
    <T>(id: string, fixture: ComponentFixture<any>): DebugElement | null =>
        fixture.debugElement.query(By.css(`.${id}`));

export const getElementsByClass =
    <T>(id: string, fixture: ComponentFixture<any>): DebugElement[] | null =>
        fixture.debugElement.queryAll(By.css(`.${id}`));