import { Injectable } from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {User} from '../../models/user';
import {UserBoard} from '../../models/user-board';

@Injectable({
  providedIn: 'root'
})
export class UserBoardCommunicationService {
  private userBoardAddSubject = new BehaviorSubject<UserBoard | null>(null);
  addUserBoard$ = this.userBoardAddSubject.asObservable();

  setAddUserBoard(userBoard: UserBoard) {
    this.userBoardAddSubject.next(userBoard);
  }

  private userBoardUpdateSubject = new BehaviorSubject<UserBoard | null>(null);
  updateUserBoard$ = this.userBoardUpdateSubject.asObservable();

  setUpdateUserBoard(userBoard: UserBoard) {
    this.userBoardUpdateSubject.next(userBoard);
  }

  private userBoardDeleteSubject = new BehaviorSubject<{ userId: number, boardId: number } | null>(null);
  deleteUserBoard$ = this.userBoardDeleteSubject.asObservable();

  setDeleteUserBoard(userId: number, boardId: number ) {
    this.userBoardDeleteSubject.next({ userId, boardId});
  }
}
