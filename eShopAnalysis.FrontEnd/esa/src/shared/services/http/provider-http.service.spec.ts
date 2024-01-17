import { TestBed } from '@angular/core/testing';

import { ProviderHttpService } from './provider-http.service';

describe('ProviderHttpService', () => {
  let service: ProviderHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ProviderHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
