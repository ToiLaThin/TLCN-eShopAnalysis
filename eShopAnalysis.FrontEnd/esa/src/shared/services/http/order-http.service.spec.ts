import { TestBed } from '@angular/core/testing';

import { OrderHttpService } from './order-http.service';

describe('OrderHttpService', () => {
  let service: OrderHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OrderHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
