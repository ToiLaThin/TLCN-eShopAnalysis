import { TestBed } from '@angular/core/testing';

import { LikeProductHttpService } from './like-product-http.service';

describe('LikeProductHttpService', () => {
  let service: LikeProductHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LikeProductHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
