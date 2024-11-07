import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataPickerComponent } from './data-picker.component';

describe('DataPickerComponent', () => {
  let component: DataPickerComponent;
  let fixture: ComponentFixture<DataPickerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DataPickerComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DataPickerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
