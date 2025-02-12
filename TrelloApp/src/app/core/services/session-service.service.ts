import { Injectable } from '@angular/core';
import {BehaviorSubject, Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SessionServiceService {

  private storage: Storage = sessionStorage;
  private themeSubject: BehaviorSubject<string>;
  private usernameSubject: BehaviorSubject<string>;

  constructor() {
    const initialTheme: string = this.getUserData()?.theme || 'light';
    this.themeSubject = new BehaviorSubject<string>(initialTheme);
    const initialUsername: string = this.getUserData()?.username || 'unnamed';
    this.usernameSubject = new BehaviorSubject<string>(initialUsername);
  }

  getTheme(): Observable<string> {
    return this.themeSubject.asObservable();
  }

  getUsername(): Observable<string> {
    return this.usernameSubject.asObservable();
  }

  getUserData(): { email: string; username: string; theme: string } | null {
    const data: string | null = this.storage.getItem('userData');
    return data ? JSON.parse(data) : null;
  }

  setUserData(data: { email: string; username: string; theme: string }): void {
    this.storage.setItem('userData', JSON.stringify(data));
    this.usernameSubject.next(data.username);
    this.themeSubject.next(data.theme);
    this.applyTheme(data.theme);
  }

  getToken(): string | null {
    const data: string | null = this.storage.getItem('token');
    return data ? JSON.parse(data) : null;
  }

  setToken(newToken: string): void {
    this.storage.setItem('token', JSON.stringify(newToken));
  }

  clearSession(): void {
    this.storage.removeItem('userData');
    this.storage.removeItem('token');
  }

  applyTheme(theme: string): void {
    const isDark: boolean = theme === 'dark';
    const body: HTMLElement = document.querySelector('body') as HTMLElement;
    body.classList.toggle('dark', isDark);
    body.classList.toggle('light', !isDark);
  }
}
