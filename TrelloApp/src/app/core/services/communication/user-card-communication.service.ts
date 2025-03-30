import { Injectable } from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {UserCard} from '../../models/user-card';

@Injectable({
  providedIn: 'root'
})
export class UserCardCommunicationService {
  private userCardAddSubject = new BehaviorSubject<UserCard | null>(null);
  addUserCard$ = this.userCardAddSubject.asObservable();

  setAddUserCard(userCard: UserCard) {
    this.userCardAddSubject.next(userCard);
  }

  private userCardDeleteSubject = new BehaviorSubject<{ userId: number, cardId: number } | null>(null);
  deleteUserCard$ = this.userCardDeleteSubject.asObservable();

  setDeleteUserCard(userId: number, cardId: number) {
    this.userCardDeleteSubject.next({userId, cardId});
  }
}
