import {Component, Input} from '@angular/core';
import {CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray, transferArrayItem} from "@angular/cdk/drag-drop";
import {NgForOf, NgIf} from "@angular/common";
import {CardItemComponent} from '../card-item/card-item.component';
import {List} from '../../../core/models/list';
import {Card, UpdateCard} from '../../../core/models/card';
import {CardHttpService} from '../../../core/services/http/card-http.service';
import {CardCommunicationService} from '../../../core/services/communication/card-communication.service';

@Component({
  selector: 'app-card-list',
  imports: [
    CdkDrag,
    CdkDropList,
    NgForOf,
    CardItemComponent,
    NgIf,
  ],
  templateUrl: './card-list.component.html',
  styleUrl: './card-list.component.css'
})
export class CardListComponent {
  @Input() lists: List[] = [];
  @Input() list: List | undefined = undefined;

  constructor(private cardHttpService: CardHttpService,
              private cardCommunicationService: CardCommunicationService) { }

  ngOnInit(): void {
    this.getCards();
    this.cardCommunicationService.addCard$.subscribe((card: Card | null): void => {
      if (card != null && this.list != undefined && this.list.id == card.listId) {
        this.list.cards.push(card);
        this.list.cards = this.list.cards.sort((a, b) => a.position - b.position);
      }
    });
    this.cardCommunicationService.deleteCard$.subscribe((cardId: number | null): void => {
      if (cardId != null && this.list != undefined) {
        this.list.cards = this.list.cards.filter(c => c.id != cardId);
        this.list.cards = this.list.cards.sort((a, b) => a.position - b.position);
      }
    });
    this.cardCommunicationService.updateCard$.subscribe((card: Card | null): void => {
      if (card != null && this.list != undefined) {
        const actualCardIndex = this.list.cards.findIndex(c => c.id === card.id);
        if (actualCardIndex !== -1) {
          const actualCard = this.list.cards[actualCardIndex];
          if (card.listId !== actualCard.listId) {
            this.list.cards = this.list.cards.filter(c => c.id !== card.id);
            const newList = this.lists.find(l => l.id === card.listId);
            if (newList) {
              newList.cards.push(card);
              newList.cards.sort((a, b) => a.position - b.position);
            }
          } else {
            this.list.cards = this.list.cards.map(c => c.id === card.id ? card : c);
          }
          this.list.cards.sort((a, b) => a.position - b.position);
        }
      }
    });

  }

  getCards(): void {
    if (this.list != undefined) {
      this.cardHttpService.getCardsByListId(this.list.id).subscribe({
        next: (result: Card[]): void => {
          if (this.list != undefined) {
            this.list.cards = result.sort((a, b) => a.position - b.position);
          }
        }
      });
    }
  }

  dropCards(event: CdkDragDrop<Card[]>): void {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex);
    }

    const previousList = this.lists.find(list => list.cards === event.previousContainer.data);
    const currentList = this.lists.find(list => list.cards === event.container.data);

    if (previousList) {
      this.onSubmitUpdateCardsPositions(previousList);
    }
    if (currentList && currentList !== previousList) {
      this.onSubmitUpdateCardsPositions(currentList);
    }
  }

  onSubmitUpdateCardsPositions(list: List): void {
    list.cards.forEach((card: Card, index: number) => {
      const body: UpdateCard = {
        listId: list.id,
        position: index
      }

      this.cardHttpService.update(card.id, body).subscribe();
    });
  }
}
