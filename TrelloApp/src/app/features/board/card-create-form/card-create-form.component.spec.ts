import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CardCreateFormComponent } from './card-create-form.component';

describe('CardCreateFormComponent', () => {
  let component: CardCreateFormComponent;
  let fixture: ComponentFixture<CardCreateFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CardCreateFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CardCreateFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
