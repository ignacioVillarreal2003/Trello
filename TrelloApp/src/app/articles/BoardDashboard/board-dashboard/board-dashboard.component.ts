import { Component } from '@angular/core';
import {NgForOf, NgIf} from '@angular/common';
import {HeaderComponent} from '../../header/header.component';
import {BoardHttpService} from '../../../core/services/http/board-http.service';
import {AlertService} from '../../../core/services/alert.service';
import {Board} from '../../../core/models/board';
import {Router} from '@angular/router';
import {CommunicationService} from '../../../core/services/communication.service';
import {CreateBoardModalComponent} from '../create-board-modal/create-board-modal.component';
import {BtnComponent} from '../../../shared/components/btn/btn.component';
import {ResourcesService} from '../../../core/services/resources.service';
import {Subject, takeUntil} from 'rxjs';
import {SessionService} from '../../../core/services/session/session.service';

@Component({
  selector: 'app-board-dashboard',
  imports: [
    NgIf,
    HeaderComponent,
    NgForOf,
    CreateBoardModalComponent,
    BtnComponent
  ],
  templateUrl: './board-dashboard.component.html',
  standalone: true,
  styleUrl: './board-dashboard.component.css'
})
export class BoardDashboardComponent {
  isCreateBoardModalOpen: boolean = false;
  boards: Board[] = [];
  boardBackgroundPath: string | undefined = undefined;
  destroy = new Subject<void>();

  constructor(private boardHttp: BoardHttpService,
              private communicationService: CommunicationService,
              private sessionService: SessionService,
              private alertService: AlertService,
              private router: Router,
              private resourcesService: ResourcesService) {}

  ngOnInit(): void {
    this.getBoards();
    this.communicationService.refreshBoards$.pipe(
      takeUntil(this.destroy)).subscribe((): void => {
        this.getBoards();
      });
    this.boardBackgroundPath = this.resourcesService.boardBackgroundPath;
  }

  ngOnDestroy(): void {
    this.destroy.next();
    this.destroy.complete();
  }

  openCreateBoardModal(): void {
    this.isCreateBoardModalOpen = true;
  }

  closeCreateBoardModal(): void {
    this.isCreateBoardModalOpen = false;
  }

  getBoards(): void {
    this.boardHttp.getBoards().subscribe({
      next: (response: Board[]): void => {
        this.boards = response;
      },
      error: (error: Error): void => {
        this.alertService.ErrorMessage(error.message);
      },
    });
  }

  openBoard(id: number) {
    this.sessionService.setBoardData({
      boardId: id,
    })
    this.router.navigate([`/board-view/${id}`]);
  }
}
