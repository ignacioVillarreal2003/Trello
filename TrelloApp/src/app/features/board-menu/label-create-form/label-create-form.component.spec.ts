import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LabelCreateFormComponent } from './label-create-form.component';

describe('LabelCreateFormComponent', () => {
  let component: LabelCreateFormComponent;
  let fixture: ComponentFixture<LabelCreateFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LabelCreateFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LabelCreateFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
