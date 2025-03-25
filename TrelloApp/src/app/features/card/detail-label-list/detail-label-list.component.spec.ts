import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailLabelListComponent } from './detail-label-list.component';

describe('DetailLabelListComponent', () => {
  let component: DetailLabelListComponent;
  let fixture: ComponentFixture<DetailLabelListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DetailLabelListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DetailLabelListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
