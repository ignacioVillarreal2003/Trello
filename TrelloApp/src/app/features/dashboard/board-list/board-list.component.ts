import { Component } from '@angular/core';
import {NgForOf} from "@angular/common";
import {BoardItemComponent} from '../board-item/board-item.component';
import {Board} from '../../../core/models/board';
import {Subject, takeUntil} from 'rxjs';
import {BoardHttpService} from '../../../core/services/http/board-http.service';
import {BoardCommunicationService} from '../../../core/services/communication/board-communication.service';

@Component({
  selector: 'app-board-list',
  imports: [
    NgForOf,
    BoardItemComponent
  ],
  templateUrl: './board-list.component.html',
  styleUrl: './board-list.component.css'
})
export class BoardListComponent {
  boards: Board[] = [];
  destroy: Subject<void> = new Subject<void>();

  constructor(private boardHttp: BoardHttpService,
              private boardCommunicationService: BoardCommunicationService) {}

  ngOnInit(): void {
    this.getBoards();
    this.boardCommunicationService.addBoard$.subscribe((board: Board | null): void => {
      if (board !== null) {
        this.boards.push(board);
      }
    });
  }

  getBoards(): void {
    this.boardHttp.getBoards().subscribe({
      next: (result: Board[]): void => {
        this.boards = result;
      }
    });
  }
}
