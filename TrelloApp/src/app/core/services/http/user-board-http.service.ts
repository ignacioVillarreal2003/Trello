import { Injectable } from '@angular/core';
import {BaseHttpService} from './base-http.service';
import {HttpClient} from '@angular/common/http';
import {catchError, Observable} from 'rxjs';
import {User} from '../../models/user';
import {AddUserBoard, UserBoard} from '../../models/user-board';

@Injectable({
  providedIn: 'root'
})
export class UserBoardHttpService extends BaseHttpService {

  url = `${this.baseUrl}/UserBoard`;

  constructor(http: HttpClient) {
    super(http);
  }

  getUsersByBoardId(boardId: number): Observable<User[]> {
    return this.http.get<User[]>(`${this.url}/board/${boardId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  add(boardId: number, requestBody: AddUserBoard): Observable<UserBoard> {
    return this.http.post<UserBoard>(`${this.url}/board/${boardId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  delete(boardId: number, userId: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/board/${boardId}/user/${userId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
