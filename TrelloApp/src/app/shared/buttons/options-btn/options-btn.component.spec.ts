import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OptionsBtnComponent } from './options-btn.component';

describe('OptionsBtnComponent', () => {
  let component: OptionsBtnComponent;
  let fixture: ComponentFixture<OptionsBtnComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OptionsBtnComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OptionsBtnComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
