import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable } from 'rxjs';
import {AddBoard, UpdateBoard} from '../../models/board';
import {BaseHttpService} from './base-http.service';

@Injectable({
  providedIn: 'root'
})
export class BoardHttpService extends BaseHttpService {

  url = `${this.baseUrl}/Board`;

  constructor(http: HttpClient) {
    super(http);
  }

  getBoard(id: number): Observable<any> {
    return this.http.get<any>(`${this.url}/${id}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  getBoards(): Observable<any> {
    return this.http.get<any>(`${this.url}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  add(requestBody: AddBoard): Observable<any> {
    return this.http.post<any>(`${this.url}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  update(boardId: number, requestBody: UpdateBoard): Observable<any> {
    return this.http.put<any>(`${this.url}/${boardId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  delete(boardId: number): Observable<any> {
    return this.http.delete<any>(`${this.url}/${boardId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
