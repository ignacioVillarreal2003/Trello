import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CardUpdateTitleFormComponent } from './card-update-title-form.component';

describe('CardUpdateTitleFormComponent', () => {
  let component: CardUpdateTitleFormComponent;
  let fixture: ComponentFixture<CardUpdateTitleFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CardUpdateTitleFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CardUpdateTitleFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
