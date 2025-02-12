import { Injectable } from '@angular/core';
import {HttpClient, HttpErrorResponse, HttpHeaders} from '@angular/common/http';
import {catchError, NotFoundError, Observable, of, throwError} from 'rxjs';
import {SessionServiceService} from '../session-service.service';

@Injectable({
  providedIn: 'root'
})
export class BoardHttpService {

  constructor(private http: HttpClient, private sessionServiceService: SessionServiceService) { }

  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
  baseUrl: string = 'https://localhost:44384/Board';

  private handleError(error: HttpErrorResponse) {
    console.log(error)
    return throwError(error.error);
  }

  getBoards(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  postBoard(title: string, theme: string, icon: string): Observable<any> {
    const requestBody: any = { title, theme, icon };
    return this.http.post<any>(`${this.baseUrl}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  putBoard(theme: string, boardId: number): Observable<any> {
    const requestBody: any = { theme, boardId };
    return this.http.put<any>(`${this.baseUrl}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
