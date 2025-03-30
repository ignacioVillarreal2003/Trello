import { Injectable } from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {User} from '../../models/user';

@Injectable({
  providedIn: 'root'
})
export class UserCommunicationService {
  private userUpdateSubject = new BehaviorSubject<User | null>(null);
  updateUser$ = this.userUpdateSubject.asObservable();

  setUpdateUser(User: User) {
    this.userUpdateSubject.next(User);
  }

  private userDeleteSubject = new BehaviorSubject<number | null>(null);
  deleteUser$ = this.userDeleteSubject.asObservable();

  setDeleteUser(userId: number) {
    this.userDeleteSubject.next(userId);
  }
}
