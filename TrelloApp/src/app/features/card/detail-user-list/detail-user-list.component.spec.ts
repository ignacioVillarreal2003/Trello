import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailUserListComponent } from './detail-user-list.component';

describe('DetailUserListComponent', () => {
  let component: DetailUserListComponent;
  let fixture: ComponentFixture<DetailUserListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetailUserListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetailUserListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
