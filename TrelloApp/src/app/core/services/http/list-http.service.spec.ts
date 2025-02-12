import { TestBed } from '@angular/core/testing';

import { ListHttpService } from './list-http.service';

describe('ListHttpService', () => {
  let service: ListHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ListHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
