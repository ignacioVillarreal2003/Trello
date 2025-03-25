import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {catchError, Observable} from 'rxjs';
import {AddList, UpdateList} from '../../models/list';
import {BaseHttpService} from './base-http.service';

@Injectable({
  providedIn: 'root'
})
export class ListHttpService extends BaseHttpService{

  url = `${this.baseUrl}/List`;

  constructor(http: HttpClient) {
    super(http);
  }

  getListsByBoardId(boardId: number): Observable<any> {
    return this.http.get<any>(`${this.url}/board/${boardId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  add(boardId: number, requestBody: AddList): Observable<any> {
    return this.http.post<any>(`${this.url}/board/${boardId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  update(listId: number, requestBody: UpdateList): Observable<any> {
    return this.http.put<any>(`${this.url}/${listId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  delete(listId: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${listId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
