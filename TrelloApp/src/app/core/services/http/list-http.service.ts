import { Injectable } from '@angular/core';
import {HttpClient, HttpErrorResponse, HttpHeaders} from '@angular/common/http';
import {catchError, Observable, throwError} from 'rxjs';
import {SessionServiceService} from '../session-service.service';

@Injectable({
  providedIn: 'root'
})
export class ListHttpService {

  constructor(private http: HttpClient, private sessionServiceService: SessionServiceService) { }

  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
  baseUrl: string = 'https://localhost:44384/List';

  private handleError(error: HttpErrorResponse) {
    console.log(error)
    return throwError(error.error);
  }

  postList(title: string, boardId: number): Observable<any> {
    const requestBody: any = { title, boardId };
    return this.http.post<any>(this.baseUrl, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  getLists(boardId: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/${boardId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
