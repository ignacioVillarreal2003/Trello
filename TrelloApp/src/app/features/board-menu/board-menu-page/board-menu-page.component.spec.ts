import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardMenuPageComponent } from './board-menu-page.component';

describe('BoardMenuPageComponent', () => {
  let component: BoardMenuPageComponent;
  let fixture: ComponentFixture<BoardMenuPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BoardMenuPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BoardMenuPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
