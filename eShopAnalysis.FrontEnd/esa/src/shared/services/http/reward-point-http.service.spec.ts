import { TestBed } from '@angular/core/testing';

import { RewardPointHttpService } from './reward-point-http.service';

describe('RewardPointService', () => {
  let service: RewardPointHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RewardPointHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
