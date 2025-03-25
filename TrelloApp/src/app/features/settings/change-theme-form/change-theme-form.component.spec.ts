import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeThemeFormComponent } from './change-theme-form.component';

describe('ChangeThemeFormComponent', () => {
  let component: ChangeThemeFormComponent;
  let fixture: ComponentFixture<ChangeThemeFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChangeThemeFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChangeThemeFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
