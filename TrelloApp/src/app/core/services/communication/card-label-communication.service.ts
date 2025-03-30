import { Injectable } from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {CardLabel} from '../../models/card-label';

@Injectable({
  providedIn: 'root'
})
export class CardLabelCommunicationService {
  private cardLabelAddSubject = new BehaviorSubject<CardLabel | null>(null);
  addCardLabel$ = this.cardLabelAddSubject.asObservable();

  setAddCardLabel(cardLabel: CardLabel) {
    this.cardLabelAddSubject.next(cardLabel);
  }

  private cardLabelDeleteSubject = new BehaviorSubject<{ labelId: number, cardId: number } | null>(null);
  deleteCardLabel$ = this.cardLabelDeleteSubject.asObservable();

  setDeleteCardLabel(cardId: number, labelId: number) {
    this.cardLabelDeleteSubject.next({cardId, labelId});
  }
}
