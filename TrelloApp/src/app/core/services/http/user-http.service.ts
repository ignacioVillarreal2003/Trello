import { Injectable } from '@angular/core';
import {catchError, Observable} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {LoginUser, RegisterUser, UpdateUser} from '../../models/user';
import {BaseHttpService} from './base-http.service';

@Injectable({
  providedIn: 'root'
})
export class UserHttpService extends BaseHttpService {

  url = `${this.baseUrl}/User`;

  constructor(http: HttpClient) {
    super(http);
  }

  register(email: string, username: string, password: string): Observable<any> {
    const requestBody: RegisterUser = { email, username, password };
    return this.http.post<any>(`${this.url}/register`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  login(email: string, password: string): Observable<any> {
    const requestBody: LoginUser = { email, password };
    return this.http.post<any>(`${this.url}/login`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  refreshToken(refreshToken: string): Observable<any> {
    return this.http.post<any>(`${this.url}/refresh-token`, { refreshToken }, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  getUsers(): Observable<any> {
    return this.http.get<any>(`${this.url}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  getUsersByUsername(username: string): Observable<any> {
    return this.http.get<any>(`${this.url}/username/${username}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  getUsersByCardId(cardId: number): Observable<any> {
    return this.http.get<any>(`${this.url}/card/${cardId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  updatePassword(oldPassword: string, newPassword: string): Observable<any> {
    const requestBody: UpdateUser = { oldPassword, newPassword };
    return this.http.put<any>(`${this.url}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  updateUsername(username: string): Observable<any> {
    const requestBody: UpdateUser = { username };
    return this.http.put<any>(`${this.url}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  updateTheme(theme: string): Observable<any> {
    const requestBody: UpdateUser = { theme };
    return this.http.put<any>(`${this.url}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  deleteUser(): Observable<any> {
    return this.http.delete<any>(`${this.url}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
