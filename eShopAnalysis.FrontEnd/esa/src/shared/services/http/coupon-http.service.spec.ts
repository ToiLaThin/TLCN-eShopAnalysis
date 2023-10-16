import { TestBed } from '@angular/core/testing';

import { CouponHttpService } from './coupon-http.service';

describe('CouponHttpService', () => {
  let service: CouponHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CouponHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
