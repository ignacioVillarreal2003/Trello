import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardCreateFormComponent } from './board-create-form.component';

describe('BoardCreateFormComponent', () => {
  let component: BoardCreateFormComponent;
  let fixture: ComponentFixture<BoardCreateFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BoardCreateFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BoardCreateFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
