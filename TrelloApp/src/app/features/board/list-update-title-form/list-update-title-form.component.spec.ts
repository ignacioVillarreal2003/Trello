import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListUpdateTitleFormComponent } from './list-update-title-form.component';

describe('ListUpdateTitleFormComponent', () => {
  let component: ListUpdateTitleFormComponent;
  let fixture: ComponentFixture<ListUpdateTitleFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListUpdateTitleFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ListUpdateTitleFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
