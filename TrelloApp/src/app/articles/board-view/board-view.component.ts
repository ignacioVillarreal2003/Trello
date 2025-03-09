import {Component} from '@angular/core';
import { HeaderComponent } from '../header/header.component';
import { NgForOf, NgIf } from '@angular/common';
import { BoardMenuComponent } from './board-menu/board-menu.component';
import { List } from '../../core/models/list';
import { ActivatedRoute } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { ListHttpService } from '../../core/services/http/list-http.service';
import { AlertService } from '../../core/services/alert.service';
import { CommunicationService } from '../../core/services/communication.service';
import {DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem, CdkDropListGroup} from '@angular/cdk/drag-drop';
import {Card} from '../../core/models/card';
import {CardViewComponent} from '../card-view/card-view.component';
import {BtnMenuComponent} from '../../shared/components/btn-menu/btn-menu.component';
import {Board} from '../../core/models/board';
import {BoardHttpService} from '../../core/services/http/board-http.service';
import {ResourcesService} from '../../core/services/resources/resources.service';
import {CreateListModalComponent} from './create-list-modal/create-list-modal.component';
import {CreateCardModalComponent} from './create-card-modal/create-card-modal.component';

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

  constructor(private route: ActivatedRoute,
    private listHttpService: ListHttpService,
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
        next: (response: any): void => {
          this.lists = response;
        },
        error: (error: HttpErrorResponse): void => {
          const errorMessage: string = error?.message || 'Error in the server. Try again later.';
          this.alertService.ErrorMessage(errorMessage);
        },
      });
    }
  }

  drop(event: CdkDragDrop<any[]>): void {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex)
    } else {
      transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex)
    }
  }
}
