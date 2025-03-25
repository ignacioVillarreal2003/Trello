import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CardUpdateIsCompletedFormComponent } from './card-update-is-completed-form.component';

describe('CardUpdateIsCompletedFormComponent', () => {
  let component: CardUpdateIsCompletedFormComponent;
  let fixture: ComponentFixture<CardUpdateIsCompletedFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CardUpdateIsCompletedFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CardUpdateIsCompletedFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
