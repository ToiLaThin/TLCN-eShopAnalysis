import { TestBed } from '@angular/core/testing';

import { ShowLoaderService } from './show-loader.service';

describe('ShowLoaderService', () => {
  let service: ShowLoaderService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ShowLoaderService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
