import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeAvatarFormComponent } from './change-avatar-form.component';

describe('ChangeAvatarFormComponent', () => {
  let component: ChangeAvatarFormComponent;
  let fixture: ComponentFixture<ChangeAvatarFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChangeAvatarFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChangeAvatarFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
