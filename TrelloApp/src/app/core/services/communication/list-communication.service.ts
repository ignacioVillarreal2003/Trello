import { Injectable } from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {List} from '../../models/list';

@Injectable({
  providedIn: 'root'
})
export class ListCommunicationService {
  private listAddSubject = new BehaviorSubject<List | null>(null);
  addList$ = this.listAddSubject.asObservable();

  setAddList(List: List) {
    this.listAddSubject.next(List);
  }

  private listUpdateSubject = new BehaviorSubject<List | null>(null);
  updateList$ = this.listUpdateSubject.asObservable();

  setUpdateList(List: List) {
    this.listUpdateSubject.next(List);
  }

  private listDeleteSubject = new BehaviorSubject<number | null>(null);
  deleteList$ = this.listDeleteSubject.asObservable();

  setDeleteList(listId: number) {
    this.listDeleteSubject.next(listId);
  }
}
