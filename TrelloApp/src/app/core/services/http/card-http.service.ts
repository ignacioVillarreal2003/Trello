import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {catchError, Observable} from 'rxjs';
import {AddCard, Card, UpdateCard} from '../../models/card';
import {BaseHttpService} from './base-http.service';

@Injectable({
  providedIn: 'root'
})
export class CardHttpService  extends BaseHttpService {
  url = `${this.baseUrl}/Card`;

  constructor(http: HttpClient) {
    super(http);
  }

  getCardById(cardId: number): Observable<Card> {
    return this.http.get<Card>(`${this.url}/${cardId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  getCardsByListId(listId: number): Observable<Card[]> {
    return this.http.get<Card[]>(`${this.url}/list/${listId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  getPriorities(): Observable<string[]> {
    return this.http.get<string[]>(`${this.url}/priorities`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  add(listId: number, requestBody: AddCard): Observable<Card> {
    return this.http.post<Card>(`${this.url}/list/${listId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  update(cardId: number, requestBody: UpdateCard): Observable<Card> {
    return this.http.put<Card>(`${this.url}/${cardId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  delete(cardId: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${cardId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
