import {Component, ElementRef, ViewChild} from '@angular/core';
import { HeaderComponent } from '../header/header.component';
import { NgForOf, NgIf, NgStyle } from '@angular/common';
import { TaskViewComponent } from '../task-view/task-view.component';
import { BoardMenuComponent } from '../board-menu/board-menu.component';
import { OptionsBtnComponent } from '../../shared/buttons/options-btn/options-btn.component';
import { CloseBtnComponent } from '../../shared/buttons/close-btn/close-btn.component';
import { CreateTaskModalComponent } from '../modals/create-task-modal/create-task-modal.component';
import { List } from '../../core/models/list';
import { ActivatedRoute } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { ListHttpService } from '../../core/services/http/list-http.service';
import { AlertService } from '../../core/services/alert.service';
import { CreateListModalComponent } from '../modals/create-list-modal/create-list-modal.component';
import { CommunicationService } from '../../core/services/communication.service';
import { CdkDragMove, DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem, CdkDropListGroup} from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-board-view',
  imports: [
    HeaderComponent,
    NgStyle,
    NgIf,
    TaskViewComponent,
    BoardMenuComponent,
    OptionsBtnComponent,
    CloseBtnComponent,
    CreateTaskModalComponent,
    NgForOf,
    CreateListModalComponent,
    DragDropModule,
    CdkDropListGroup,
  ],
  templateUrl: './board-view.component.html',
  standalone: true,
  styleUrl: './board-view.component.css'
})
export class BoardViewComponent {
  isOpenCreateList: boolean = false;
  boardId: number | undefined = undefined;
  isOpenCreateTask: boolean = false;
  listId: number | undefined = undefined;
  isOpenTaskView: boolean = false;
  isOpenBoardMenu: boolean = false;
  lists: List[] = [
    {
      title: "Lista 1",
      tasks: [
        {
          title: "Lista 1 - Tarea 1",
        },
        {
          title: "Lista 1 - Tarea 2",
        },
        {
          title: "Lista 1 - Tarea 3",
        }
      ]
    },
    {
      title: "Lista 2",
      tasks: [
        {
          title: "Lista 2 - Tarea 1",
        },
        {
          title: "Lista 2 - Tarea 2",
        },
        {
          title: "Lista 2 - Tarea 3",
        }
      ]
    },
    {
      title: "Lista 2",
      tasks: [
        {
          title: "Lista 2 - Tarea 1",
        },
        {
          title: "Lista 2 - Tarea 2",
        },
        {
          title: "Lista 2 - Tarea 3",
        }
      ]
    }
  ]

  drop(event: CdkDragDrop<any[]>): void {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex)
    } else {
      transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex)
    }
    console.log(this.lists)
  }

  constructor(private route: ActivatedRoute,
    private listHttpService: ListHttpService,
    private alertService: AlertService,
    private communicationService: CommunicationService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params: any): void => {
      this.boardId = params.get('id');
      this.getLists();
    });
    this.communicationService.refreshLists$.subscribe((): void => {
      this.getLists();
    })
  }

  getLists(): void {
    if (this.boardId) {
      this.listHttpService.getLists(this.boardId).subscribe({
        next: (response: any): void => {
          this.lists = response.lists.$values;
        },
        error: (error: HttpErrorResponse): void => {
          const errorMessage: string = error?.message || 'Error in the server. Try again later.';
          this.alertService.ErrorMessage(errorMessage);
        },
      });
    }
  }

  switchCreateList(): void {
    this.isOpenCreateList = !this.isOpenCreateList;
    const body: HTMLElement = document.querySelector('body') as HTMLElement;
    if (!this.isOpenCreateList) {
      body.style.overflow = 'auto';
    } else {
      body.style.overflow = 'hidden';
    }
  }

  switchCreateTask(listId: number | undefined): void {
    this.listId = listId;
    this.isOpenCreateTask = !this.isOpenCreateTask;
    const body: HTMLElement = document.querySelector('body') as HTMLElement;
    if (!this.isOpenCreateTask) {
      body.style.overflow = 'auto';
    } else {
      body.style.overflow = 'hidden';
    }
  }

  switchTaskView(listId: number | undefined): void {
    this.listId = listId;
    this.isOpenTaskView = !this.isOpenTaskView;
    const body: HTMLElement = document.querySelector('body') as HTMLElement;
    if (!this.isOpenTaskView) {
      body.style.overflow = 'auto';
    } else {
      body.style.overflow = 'hidden';
    }
  }

  switchBoardMenu(): void {
    this.isOpenBoardMenu = !this.isOpenBoardMenu;
  }

  addList() {

  }

  addTask(n: string | undefined) {

  }
}
