import { Injectable } from '@angular/core';
import {HttpClient, HttpErrorResponse, HttpHeaders} from '@angular/common/http';
import {catchError, Observable, throwError} from 'rxjs';
import {SessionServiceService} from '../session/session-service.service';
import {AddCard} from '../../models/card';

@Injectable({
  providedIn: 'root'
})
export class CardHttpService {

  constructor(private http: HttpClient, private sessionServiceService: SessionServiceService) { }

  httpOptions = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
  baseUrl: string = 'http://localhost:5182/Card';

  private handleError(error: HttpErrorResponse) {
    console.log(error)
    return throwError(error.error);
  }

  postCard(title: string, description: string, listId: number): Observable<any> {
    const requestBody: AddCard = {
      title: title,
      description: description
    };
    return this.http.post<any>(`${this.baseUrl}/list/${listId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
