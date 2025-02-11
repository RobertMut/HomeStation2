import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import {MatSidenavModule} from "@angular/material/sidenav";
import {NoopAnimationsModule} from "@angular/platform-browser/animations";
import {MatIconModule} from "@angular/material/icon";
import {CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA} from "@angular/core";
import {getManyItemsByTestSelector} from "./tests/utils";
import {RouterTestingModule} from "@angular/router/testing";

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppComponent],
      imports: [HttpClientTestingModule, MatSidenavModule, NoopAnimationsModule, MatIconModule, 
        RouterTestingModule.withRoutes([
          {path: 'current', component: AppComponent},
          {path: 'temperature', component: AppComponent},
          {path: 'pressure', component: AppComponent},
          {path: 'air-quality', component: AppComponent}
        ])],
      schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create the app', () => {
    expect(component).toBeTruthy();
  });

  it('should contain navigation labels', () => {
    fixture.detectChanges();
    
    let labels = getManyItemsByTestSelector('label', fixture)?.map(x => x.nativeElement.textContent);
    
    expect(labels).toEqual([" Current ", " Temperature and Humidity ", " Pressure ", " Air Quality "]);
  });
});
