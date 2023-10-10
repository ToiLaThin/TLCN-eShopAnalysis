import { TestBed } from '@angular/core/testing';

import { SaleHttpService } from './sale-http.service';

describe('SaleHttpService', () => {
  let service: SaleHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SaleHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
