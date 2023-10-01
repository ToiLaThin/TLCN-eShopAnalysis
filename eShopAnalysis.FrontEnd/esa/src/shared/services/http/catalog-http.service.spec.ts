import { TestBed } from '@angular/core/testing';

import { CatalogHttpService } from './catalog-http.service';

describe('CatalogHttpService', () => {
  let service: CatalogHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CatalogHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
