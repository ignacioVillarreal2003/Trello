import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BtnThemeComponent } from './btn-theme.component';

describe('BtnThemeComponent', () => {
  let component: BtnThemeComponent;
  let fixture: ComponentFixture<BtnThemeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BtnThemeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BtnThemeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
