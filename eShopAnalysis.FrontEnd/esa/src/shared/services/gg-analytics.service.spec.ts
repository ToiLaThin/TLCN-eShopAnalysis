import { TestBed } from '@angular/core/testing';

import { GgAnalyticsService } from './gg-analytics.service';

describe('GgAnalyticsService', () => {
  let service: GgAnalyticsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GgAnalyticsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
