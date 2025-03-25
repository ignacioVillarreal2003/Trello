import { Injectable } from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {User} from '../../models/user';

@Injectable({
  providedIn: 'root'
})
export class UserCommunicationService {
  private userAddSubject = new BehaviorSubject<User | null>(null);
  addUser$ = this.userAddSubject.asObservable();

  setAddUser(User: User) {
    this.userAddSubject.next(User);
  }

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
