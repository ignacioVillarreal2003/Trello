import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BtnDeleteCardComponent } from './btn-delete-card.component';

describe('BtnDeleteCardComponent', () => {
  let component: BtnDeleteCardComponent;
  let fixture: ComponentFixture<BtnDeleteCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BtnDeleteCardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BtnDeleteCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
