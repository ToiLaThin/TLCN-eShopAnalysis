import { TestBed } from '@angular/core/testing';

import { CustomerOrderAddressService } from './customer-order-address.service';

describe('CustomerOrderAddressService', () => {
  let service: CustomerOrderAddressService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CustomerOrderAddressService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
