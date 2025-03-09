import { Injectable } from '@angular/core';
import {HttpClient, HttpErrorResponse, HttpHeaders} from '@angular/common/http';
import {catchError, Observable, throwError} from 'rxjs';
import {SessionServiceService} from '../session/session-service.service';
import {AddList, List} from '../../models/list';

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

  postList(title: string, position: number, boardId: number): Observable<any> {
    const requestBody: AddList = {
      title: title,
      position: position
    };
    console.log(requestBody)
    return this.http.post<any>(`${this.baseUrl}/board/${boardId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  getLists(boardId: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/board/${boardId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
