import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardUpdateTitleFormComponent } from './board-update-title-form.component';

describe('BoardUpdateTitleFormComponent', () => {
  let component: BoardUpdateTitleFormComponent;
  let fixture: ComponentFixture<BoardUpdateTitleFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BoardUpdateTitleFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BoardUpdateTitleFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
