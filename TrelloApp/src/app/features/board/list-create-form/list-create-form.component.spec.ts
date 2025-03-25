import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListCreateFormComponent } from './list-create-form.component';

describe('ListCreateFormComponent', () => {
  let component: ListCreateFormComponent;
  let fixture: ComponentFixture<ListCreateFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListCreateFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ListCreateFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
