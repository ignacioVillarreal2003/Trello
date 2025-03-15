import { Injectable } from '@angular/core';
import {catchError, Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {LoginUser, RegisterUser, UpdateUser, User, UserAuth} from '../../models/user';
import {BaseHttpService} from './base-http.service';

@Injectable({
  providedIn: 'root'
})
export class UserHttpService extends BaseHttpService {

  url = `${this.baseUrl}/User`;

  constructor(http: HttpClient) {
    super(http);
  }

  register(requestBody: RegisterUser): Observable<UserAuth> {
    return this.http.post<UserAuth>(`${this.url}/register`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  login(requestBody: LoginUser): Observable<UserAuth> {
    return this.http.post<UserAuth>(`${this.url}/login`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  refreshToken(refreshToken: string): Observable<string> {
    return this.http.post<string>(`${this.url}/refresh-token`, { refreshToken }, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.url}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  getUsersByUsername(username: string): Observable<User[]> {
    console.log(`${this.url}/username/${username}`)
    return this.http.get<User[]>(`${this.url}/username/${username}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  getUsersByCardId(cardId: number): Observable<User[]> {
    return this.http.get<User[]>(`${this.url}/card/${cardId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  update(requestBody: UpdateUser): Observable<User> {
    return this.http.put<User>(`${this.url}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  delete(): Observable<void> {
    return this.http.delete<void>(`${this.url}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
