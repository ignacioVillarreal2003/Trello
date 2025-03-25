import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MemberAddFormComponent } from './member-add-form.component';

describe('MemberAddFormComponent', () => {
  let component: MemberAddFormComponent;
  let fixture: ComponentFixture<MemberAddFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MemberAddFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MemberAddFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
