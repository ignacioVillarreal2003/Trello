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

  addBoard(title: string, description: string, background: string): Observable<any> {
    const requestBody: AddBoard = { title, description, background };
    return this.http.post<any>(`${this.url}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  updateBackground(boardId: number, background: string): Observable<any> {
    const requestBody: UpdateBoard = {
      background: background
    };
    return this.http.put<any>(`${this.url}/${boardId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  updateDescription(boardId: number, description: string): Observable<any> {
    const requestBody: UpdateBoard = {
      description: description
    };
    return this.http.put<any>(`${this.url}/${boardId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  updateTitle(boardId: number, title: string): Observable<any> {
    const requestBody: UpdateBoard = {
      title: title
    };
    return this.http.patch<any>(`${this.url}/${boardId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  updateIsArchived(boardId: number, isArchived: boolean): Observable<any> {
    const requestBody: UpdateBoard = {
      isArchived: isArchived
    };
    return this.http.patch<any>(`${this.url}/${boardId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  deleteBoard(boardId: number): Observable<any> {
    return this.http.delete<any>(`${this.url}/${boardId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
