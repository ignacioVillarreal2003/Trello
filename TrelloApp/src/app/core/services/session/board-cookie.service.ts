import { Injectable } from '@angular/core';
import {CookieService} from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class BoardCookieService {

  constructor(private cookieService: CookieService) {}

  setCookie(value: number): void {
    const cookieOptions: any = { expires: 1, path: '/' };

    if (window.location.hostname === 'localhost') {
      cookieOptions.domain = 'localhost';
    }

    this.cookieService.set("BoardId", value.toString(), cookieOptions);
  }

  getCookie(): number | undefined {
    const savedBoard: string = this.cookieService.get("BoardId");
    if (savedBoard) {
      const boardId: number = Number(savedBoard);
      return isNaN(boardId) ? undefined : boardId;
    }
    return undefined;
  }

  deleteCookie(): void {
    this.cookieService.delete("BoardId");
  }
}
