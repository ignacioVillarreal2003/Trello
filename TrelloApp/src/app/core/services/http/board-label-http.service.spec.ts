import { TestBed } from '@angular/core/testing';

import { BoardLabelHttpService } from './board-label-http.service';

describe('BoardLabelHttpService', () => {
  let service: BoardLabelHttpService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BoardLabelHttpService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
