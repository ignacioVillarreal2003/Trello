import { Injectable } from '@angular/core';
import {BehaviorSubject, Observable} from 'rxjs';
import {BoardData, SessionData} from './session-data';

@Injectable({
  providedIn: 'root'
})
export class SessionService {

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

  getBoardData(): BoardData | undefined {
    const data: string | null = this.storage.getItem('boardData');
    return data ? JSON.parse(data) : undefined;
  }

  setBoardData(data: BoardData): void {
    this.storage.setItem('boardData', JSON.stringify(data));
  }

  clearSession(): void {
    this.storage.removeItem('userData');
    this.storage.removeItem('boardData');
  }
}
