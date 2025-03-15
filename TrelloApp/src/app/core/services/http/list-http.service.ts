import { Injectable } from '@angular/core';
import {HttpClient, HttpErrorResponse, HttpHeaders} from '@angular/common/http';
import {catchError, Observable, throwError} from 'rxjs';
import {SessionService} from '../session/session.service';
import {AddList, List, UpdateList} from '../../models/list';

@Injectable({
  providedIn: 'root'
})
export class ListHttpService {

  constructor(private http: HttpClient) { }

  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
  baseUrl: string = 'http://localhost:5182/List';

  private handleError(error: HttpErrorResponse) {
    console.log(error)
    return throwError(error.error);
  }

  getLists(boardId: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/board/${boardId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  add(boardId: number, requestBody: AddList): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/board/${boardId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  update(listId: number, requestBody: UpdateList): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/${listId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
