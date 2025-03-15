import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BtnDeleteComponent } from './btn-delete.component';

describe('BtnDeleteComponent', () => {
  let component: BtnDeleteComponent;
  let fixture: ComponentFixture<BtnDeleteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BtnDeleteComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BtnDeleteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
