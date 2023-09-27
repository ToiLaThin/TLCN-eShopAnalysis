import { TestBed } from '@angular/core/testing';

import { ShowDropDownService } from './show-drop-down.service';

describe('ShowDropDownService', () => {
  let service: ShowDropDownService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ShowDropDownService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
