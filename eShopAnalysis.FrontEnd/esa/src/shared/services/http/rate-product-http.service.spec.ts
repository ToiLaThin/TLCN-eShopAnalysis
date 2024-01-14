import { TestBed } from '@angular/core/testing';

import { RateProductHttpService } from './rate-product-http.service';

describe('RateProductHttpService', () => {
  let service: RateProductHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RateProductHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
