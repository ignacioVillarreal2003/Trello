import {Component} from '@angular/core';
import { HeaderComponent } from '../header/header.component';
import { NgForOf, NgIf } from '@angular/common';
import { BoardMenuComponent } from '../BoardMenu/board-menu/board-menu.component';
import {AddList, List, UpdateList} from '../../core/models/list';
import {ActivatedRoute, Router} from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { ListHttpService } from '../../core/services/http/list-http.service';
import { AlertService } from '../../core/services/alert.service';
import { CommunicationService } from '../../core/services/communication.service';
import {DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem, CdkDropListGroup} from '@angular/cdk/drag-drop';
import {Card, UpdateCard} from '../../core/models/card';
import {CardViewComponent} from '../CardView/card-view/card-view.component';
import {BtnMenuComponent} from '../../shared/components/btn-menu/btn-menu.component';
import {Board, UpdateBoard} from '../../core/models/board';
import {BoardHttpService} from '../../core/services/http/board-http.service';
import {ResourcesService} from '../../core/services/resources.service';
import {CreateListModalComponent} from './create-list-modal/create-list-modal.component';
import {CreateCardModalComponent} from './create-card-modal/create-card-modal.component';
import {CardHttpService} from '../../core/services/http/card-http.service';

@Component({
  selector: 'app-board-view',
  imports: [
    HeaderComponent,
    BoardMenuComponent,
    NgForOf,
    DragDropModule,
    CdkDropListGroup,
    CardViewComponent,
    NgIf,
    BtnMenuComponent,
    CreateListModalComponent,
    CreateCardModalComponent,
  ],
  templateUrl: './board-view.component.html',
  standalone: true,
  styleUrl: './board-view.component.css'
})
export class BoardViewComponent {
  boardId: number | undefined = undefined;
  board: Board | undefined = undefined;
  boardBackgroundPath: string = "";
  lists: List[] = [];
  isOpenCreateList: boolean = false;
  list: List | undefined = undefined;
  isOpenCreateCard: boolean = false;
  isOpenBoardMenu: boolean = false;
  cardsCount: number = 0;

  constructor(private route: ActivatedRoute,
    private router: Router,
    private listHttpService: ListHttpService,
    private cardHttpService: CardHttpService,
    private boardHttpService: BoardHttpService,
    private resourcesService: ResourcesService,
    private alertService: AlertService,
    private communicationService: CommunicationService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params: any): void => {
      this.boardId = params.get('id');
      this.getBoard();
      this.getLists();
    });
    this.communicationService.refreshLists$.subscribe((): void => {
      this.getLists();
    })
    this.communicationService.refreshCards$.subscribe((): void => {
      if (this.list !== undefined) {
        this.getCards(this.list);
      }
    })
    this.boardBackgroundPath = this.resourcesService.boardBackgroundPath;
  }

  getBoard(): void {
    if (this.boardId) {
      this.boardHttpService.getBoard(this.boardId).subscribe({
        next: (response: any): void => {
          this.board = response;
        },
        error: (error: HttpErrorResponse): void => {
          const errorMessage: string = error?.message || 'Error in the server. Try again later.';
          this.alertService.ErrorMessage(errorMessage);
        },
      });
    }
  }

  getLists(): void {
    if (this.boardId) {
      this.listHttpService.getLists(this.boardId).subscribe({
        next: (response: List[]): void => {
          this.lists = response.sort((a, b) => a.position - b.position);
          this.lists.forEach((list: List): void => {
            this.getCards(list);
          })
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message);
        },
      });
    }
  }

  getCards(list: List): void {
    this.cardHttpService.getCardsByListId(list.id).subscribe({
      next: (response: Card[]): void => {
        list.cards = response.sort((a, b) => a.position - b.position);
      },
      error: (error: Error): void => {
        this.alertService.ErrorMessage(error.message);
      },
    });
  }

  updateBoardTitle(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    const title = inputElement.value.trim();
    if (this.board && this.boardId && title !== this.board.title) {
      const body: UpdateBoard = {
        title: title
      }
      this.boardHttpService.update(this.boardId, body).subscribe({
        next: (response: Board): void => {
          if (this.board) {
            this.board.title = response.title;
          }
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message);
        },
      });
    }
  }

  dropLists(event: CdkDragDrop<any[]>): void {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex)
    }
    this.updateListsPositions();
  }

  updateListsPositions(): void {
    this.lists.forEach((list: List, index: number) => {
      const body: UpdateList = {
        position: index
      }
      this.listHttpService.update(list.id, body).subscribe({
        next: (updatedList: List) => {},
        error: (error: Error) => {
          this.alertService.ErrorMessage(error.message);
        },
      });
    });
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
      this.updateCardsPositions(previousList);
    }
    if (currentList && currentList !== previousList) {
      this.updateCardsPositions(currentList);
    }
  }

  updateCardsPositions(list: List): void {
    list.cards.forEach((card: Card, index: number) => {
      const body: UpdateCard = {
        listId: list.id,
        position: index
      }
      this.cardHttpService.update(card.id, body).subscribe({
        next: (response: Card) => {},
        error: (error: Error) => {
          this.alertService.ErrorMessage(error.message);
        },
      });
    });
  }

  openCreateListModal(): void {
    this.isOpenCreateList = true;
  }

  closeCreateListModal(): void {
    this.isOpenCreateList = false;
  }

  openCreateCardModal(list: List): void {
    this.isOpenCreateCard = true;
    this.list = list;
    this.cardsCount = list?.cards?.length ?? 0;
  }

  closeCreateCardModal(): void {
    this.isOpenCreateCard = false;
  }

  openCardView(id: number) {
    this.router.navigate([`card-view/${id}`])
  }
}
