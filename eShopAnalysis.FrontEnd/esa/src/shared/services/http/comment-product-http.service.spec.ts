import { TestBed } from '@angular/core/testing';

import { CommentProductHttpService } from './comment-product-http.service';

describe('CommentProductHttpService', () => {
  let service: CommentProductHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CommentProductHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
