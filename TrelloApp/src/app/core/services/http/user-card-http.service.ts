import { Injectable } from '@angular/core';
import {BaseHttpService} from './base-http.service';
import {HttpClient} from '@angular/common/http';
import {catchError, Observable} from 'rxjs';
import {User} from '../../models/user';
import {AddUserCard, UserCard} from '../../models/user-card';

@Injectable({
  providedIn: 'root'
})
export class UserCardHttpService extends BaseHttpService {

  private url = `${this.baseUrl}/UserCard`;

  constructor(http: HttpClient) {
    super(http);
  }

  getUsersByCardId(cardId: number): Observable<User[]> {
    console.log(`${this.url}/${cardId}`);
    return this.http.get<User[]>(`${this.url}/${cardId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  addUserToCard(cardId: number, requestBody: AddUserCard): Observable<UserCard> {
    return this.http.post<UserCard>(`${this.url}/card/${cardId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  removeUserFromCard(userId: number, cardId: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/user/${userId}/card/${cardId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
