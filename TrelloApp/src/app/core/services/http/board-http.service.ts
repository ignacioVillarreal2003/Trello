import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable } from 'rxjs';
import {AddBoard, Board, UpdateBoard} from '../../models/board';
import {BaseHttpService} from './base-http.service';

@Injectable({
  providedIn: 'root'
})
export class BoardHttpService extends BaseHttpService {

  url = `${this.baseUrl}/Board`;

  constructor(http: HttpClient) {
    super(http);
  }

  getBoard(id: number): Observable<Board> {
    return this.http.get<Board>(`${this.url}/${id}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  getBoards(): Observable<Board[]> {
    return this.http.get<Board[]>(`${this.url}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  add(requestBody: AddBoard): Observable<Board> {
    return this.http.post<Board>(`${this.url}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  update(boardId: number, requestBody: UpdateBoard): Observable<Board> {
    return this.http.put<Board>(`${this.url}/${boardId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  delete(boardId: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${boardId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
