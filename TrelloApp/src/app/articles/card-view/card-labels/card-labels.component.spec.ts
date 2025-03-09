import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CardLabelsComponent } from './card-labels.component';

describe('CardLabelsComponent', () => {
  let component: CardLabelsComponent;
  let fixture: ComponentFixture<CardLabelsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CardLabelsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CardLabelsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
