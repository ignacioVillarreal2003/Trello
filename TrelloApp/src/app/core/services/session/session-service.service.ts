import { Injectable } from '@angular/core';
import {BehaviorSubject, Observable} from 'rxjs';
import {SessionData} from './session-data';

@Injectable({
  providedIn: 'root'
})
export class SessionServiceService {

  private storage: Storage = sessionStorage;

  constructor() {}

  getSessionData(): SessionData | undefined {
    const data: string | null = this.storage.getItem('sessionData');
    return data ? JSON.parse(data) : undefined;
  }

  setSessionData(data: SessionData): void {
    this.storage.setItem('sessionData', JSON.stringify(data));
  }

  updateSessionData(partialData: Partial<SessionData>): void {
    const currentData = this.getSessionData() || {} as SessionData;
    const updatedData: SessionData = {
      ...currentData,
      ...partialData
    } as SessionData;
    this.setSessionData(updatedData);
  }

  clearSession(): void {
    this.storage.removeItem('userData');
  }
}
