import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleCouponComponent } from './sale-coupon.component';

describe('SaleCouponComponent', () => {
  let component: SaleCouponComponent;
  let fixture: ComponentFixture<SaleCouponComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SaleCouponComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SaleCouponComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
