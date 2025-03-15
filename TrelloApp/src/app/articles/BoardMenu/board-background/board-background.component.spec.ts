import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardBackgroundComponent } from './board-background.component';

describe('BoardBackgroundComponent', () => {
  let component: BoardBackgroundComponent;
  let fixture: ComponentFixture<BoardBackgroundComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BoardBackgroundComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BoardBackgroundComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
