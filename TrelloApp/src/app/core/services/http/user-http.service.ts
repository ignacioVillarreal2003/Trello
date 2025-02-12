import { Injectable } from '@angular/core';
import {catchError, map, Observable, of, throwError} from "rxjs";
import {HttpClient, HttpErrorResponse, HttpHeaders} from "@angular/common/http";
import {LoginUser, RegisterUser, UpdateUser, User} from '../../models/user';
import {SessionServiceService} from '../session-service.service';

@Injectable({
  providedIn: 'root'
})
export class UserHttpService {

  private baseUrl: string = 'https://localhost:44384/User';
  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private http: HttpClient, private sessionServiceService: SessionServiceService) {}

  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('API Error:', error);
    return throwError(() => new Error(error.error || 'Unexpected error occurred'));
  }

  register(email: string, username: string, password: string): Observable<any> {
    const requestBody: RegisterUser = { email: email,username: username, password: password, theme: 'light' };
    return this.http.post<any>(`${this.baseUrl}/register-user`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  login(email: string, password: string): Observable<any> {
    const requestBody: LoginUser = { email: email, password: password };
    return this.http.post<any>(`${this.baseUrl}/login-user`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  updatePassword(oldPassword: string, newPassword: string): Observable<any> {
    const requestBody: UpdateUser = { oldPassword: oldPassword, newPassword: newPassword };
    return this.http.put<any>(`${this.baseUrl}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  updateUsername(username: string): Observable<any> {
    const requestBody: UpdateUser = { username: username };
    return this.http.put<any>(`${this.baseUrl}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  updateTheme(theme: string): Observable<any> {
    const requestBody: UpdateUser = { theme: theme };
    return this.http.put<any>(`${this.baseUrl}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
