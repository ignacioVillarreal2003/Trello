import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MenuLabelComponent } from './menu-label.component';

describe('MenuLabelComponent', () => {
  let component: MenuLabelComponent;
  let fixture: ComponentFixture<MenuLabelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MenuLabelComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MenuLabelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
