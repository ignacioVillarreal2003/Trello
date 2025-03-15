import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardLabelComponent } from './board-label.component';

describe('BoardLabelComponent', () => {
  let component: BoardLabelComponent;
  let fixture: ComponentFixture<BoardLabelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BoardLabelComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BoardLabelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
