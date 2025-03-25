import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BtnDeleteListComponent } from './btn-delete-list.component';

describe('BtnDeleteListComponent', () => {
  let component: BtnDeleteListComponent;
  let fixture: ComponentFixture<BtnDeleteListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BtnDeleteListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BtnDeleteListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
