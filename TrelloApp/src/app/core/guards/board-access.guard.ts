import {CanActivateFn, Router} from '@angular/router';
import {inject} from '@angular/core';
import {BoardHttpService} from '../services/http/board-http.service';
import {BoardCookieService} from '../services/session/board-cookie.service';
import {Board} from '../models/board';
import {catchError, map, of} from 'rxjs';

export const boardAccessGuard: CanActivateFn = (route, state) => {
  const boardHttpService: BoardHttpService = inject(BoardHttpService);
  const boardCookieService: BoardCookieService = inject(BoardCookieService);
  const router: Router = inject(Router);

  const boardId: number | undefined = boardCookieService.getCookie();

  if (boardId == undefined) {
    router.navigate(['/']);
    return false;
  }

  return boardHttpService.getBoard(boardId).pipe(
    map((board: Board) => {
      return true;
    }),
    catchError((error: Error) => {
      router.navigate(['/error']);
      return of(false);
    })
  );
};
