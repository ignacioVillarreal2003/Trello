import { Injectable } from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {Card} from '../../models/card';

@Injectable({
  providedIn: 'root'
})
export class CardCommunicationService {
  private cardAddSubject = new BehaviorSubject<Card | null>(null);
  addCard$ = this.cardAddSubject.asObservable();

  setAddCard(card: Card) {
    this.cardAddSubject.next(card);
  }

  private cardUpdateSubject = new BehaviorSubject<Card | null>(null);
  updateCard$ = this.cardUpdateSubject.asObservable();

  setUpdateCard(card: Card) {
    this.cardUpdateSubject.next(card);
  }

  private cardDeleteSubject = new BehaviorSubject<number | null>(null);
  deleteCard$ = this.cardDeleteSubject.asObservable();

  setDeleteCard(cardId: number) {
    this.cardDeleteSubject.next(cardId);
  }
}
