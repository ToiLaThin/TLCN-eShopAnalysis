import { TestBed } from '@angular/core/testing';

import { BookmarkHttpService } from './bookmark-http.service';

describe('BookmarkHttpService', () => {
  let service: BookmarkHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BookmarkHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
