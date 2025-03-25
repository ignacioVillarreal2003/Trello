import { Injectable } from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {Board} from '../../models/board';

@Injectable({
  providedIn: 'root'
})
export class BoardCommunicationService {
  private boardAddSubject = new BehaviorSubject<Board | null>(null);
  addBoard$ = this.boardAddSubject.asObservable();

  setAddBoard(Board: Board) {
    this.boardAddSubject.next(Board);
  }

  private boardUpdateSubject = new BehaviorSubject<Board | null>(null);
  updateBoard$ = this.boardUpdateSubject.asObservable();

  setUpdateBoard(Board: Board) {
    this.boardUpdateSubject.next(Board);
  }

  private boardDeleteSubject = new BehaviorSubject<number | null>(null);
  deleteBoard$ = this.boardDeleteSubject.asObservable();

  setDeleteBoard(boardId: number) {
    this.boardDeleteSubject.next(boardId);
  }
}
