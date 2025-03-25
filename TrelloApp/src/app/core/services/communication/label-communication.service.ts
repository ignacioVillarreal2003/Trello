import { Injectable } from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {Label} from '../../models/label';

@Injectable({
  providedIn: 'root'
})
export class LabelCommunicationService {
  private labelAddSubject = new BehaviorSubject<Label | null>(null);
  addLabel$ = this.labelAddSubject.asObservable();

  setAddLabel(label: Label) {
    this.labelAddSubject.next(label);
  }

  private labelUpdateSubject = new BehaviorSubject<Label | null>(null);
  updateLabel$ = this.labelUpdateSubject.asObservable();

  setUpdateLabel(label: Label) {
    this.labelUpdateSubject.next(label);
  }

  private labelDeleteSubject = new BehaviorSubject<number | null>(null);
  deleteLabel$ = this.labelDeleteSubject.asObservable();

  setDeleteLabel(labelId: number) {
    this.labelDeleteSubject.next(labelId);
  }
}
