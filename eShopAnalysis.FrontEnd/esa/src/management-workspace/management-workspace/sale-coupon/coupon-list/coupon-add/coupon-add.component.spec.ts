import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CouponAddComponent } from './coupon-add.component';

describe('CouponAddComponent', () => {
  let component: CouponAddComponent;
  let fixture: ComponentFixture<CouponAddComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CouponAddComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CouponAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
