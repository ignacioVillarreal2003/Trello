import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommentCreateFormComponent } from './comment-create-form.component';

describe('CommentCreateFormComponent', () => {
  let component: CommentCreateFormComponent;
  let fixture: ComponentFixture<CommentCreateFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommentCreateFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CommentCreateFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
