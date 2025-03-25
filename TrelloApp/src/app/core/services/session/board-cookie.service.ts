import { Injectable } from '@angular/core';
import {CookieService} from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class BoardCookieService {

  constructor(private cookieService: CookieService) {}

  setCookie(value: number): void {
    this.cookieService.set("BoardId", value.toString(), { expires: 1, path: '/', domain: 'localhost' });
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
