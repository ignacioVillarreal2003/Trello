import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BackgroundListComponent } from './background-list.component';

describe('BackgroundListComponent', () => {
  let component: BackgroundListComponent;
  let fixture: ComponentFixture<BackgroundListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BackgroundListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BackgroundListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
