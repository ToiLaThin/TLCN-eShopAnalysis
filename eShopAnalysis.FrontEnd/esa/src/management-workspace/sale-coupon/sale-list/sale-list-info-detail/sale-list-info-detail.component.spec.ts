import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleListInfoDetailComponent } from './sale-list-info-detail.component';

describe('SaleListInfoDetailComponent', () => {
  let component: SaleListInfoDetailComponent;
  let fixture: ComponentFixture<SaleListInfoDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SaleListInfoDetailComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SaleListInfoDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
