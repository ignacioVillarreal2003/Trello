import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BtnDeleteBoardComponent } from './btn-delete-board.component';

describe('BtnDeleteBoardComponent', () => {
  let component: BtnDeleteBoardComponent;
  let fixture: ComponentFixture<BtnDeleteBoardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BtnDeleteBoardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BtnDeleteBoardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
