import { Component } from '@angular/core';
import {NgForOf, NgIf, NgStyle} from '@angular/common';
import {HeaderComponent} from '../../shared/header/header.component';
import {BoardHttpService} from '../../core/services/http/board-http.service';
import {HttpErrorResponse} from '@angular/common/http';
import {AlertService} from '../../core/services/alert.service';
import {Board} from '../../core/models/board';
import {Router} from '@angular/router';
import {CommunicationService} from '../../core/services/communication.service';
import {CreateBoardModalComponent} from '../modals/create-board-modal/create-board-modal.component';

@Component({
  selector: 'app-board-dashboard',
  imports: [
    NgIf,
    HeaderComponent,
    NgForOf,
    NgStyle,
    CreateBoardModalComponent
  ],
  templateUrl: './board-dashboard.component.html',
  standalone: true,
  styleUrl: './board-dashboard.component.css'
})
export class BoardDashboardComponent {
  isOpenCreateBoard: boolean = false;
  boards: Board[] = [];

  constructor(private boardHttp: BoardHttpService,
              private communicationService: CommunicationService,
              private alertService: AlertService,
              private router: Router) {}

  ngOnInit(): void {
    this.getBoards();
    this.communicationService.refreshBoards$.subscribe((): void => {
      this.getBoards();
    });
  }

  getBoards(): void {
    this.boardHttp.getBoards().subscribe({
      next: (response: any): void => {
        this.boards = response.boards.$values;
      },
      error: (error: HttpErrorResponse): void => {
        const errorMessage: string = error?.message || 'Error in the server. Try again later.';
        this.alertService.ErrorMessage(errorMessage);
      },
    });
  }

  switchCreateBoard(): void {
    this.isOpenCreateBoard = !this.isOpenCreateBoard;
    const body: HTMLElement = document.querySelector('body') as HTMLElement;
    if (!this.isOpenCreateBoard) {
      body.style.overflow = 'auto';
    } else {
      body.style.overflow = 'hidden';
    }
  }

  openBoard(id: number | undefined) {
    if (id) {
      this.router.navigate([`/board-view/${id}`]);
    }
  }
}
