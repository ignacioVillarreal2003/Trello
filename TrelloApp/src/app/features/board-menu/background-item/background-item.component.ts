import {Component, Input} from '@angular/core';
import {Board, UpdateBoard} from '../../../core/models/board';
import {BoardHttpService} from '../../../core/services/http/board-http.service';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-background-item',
  imports: [
    NgIf
  ],
  templateUrl: './background-item.component.html',
  styleUrl: './background-item.component.css'
})
export class BackgroundItemComponent {
  @Input() board: Board | undefined = undefined;
  @Input() background: string | undefined = undefined;
  @Input() backgroundPath: string | undefined = undefined;

  constructor(private boardHttpService: BoardHttpService) {}

  onSubmitUpdateBoardBackground(): void {
    if (this.board && this.background) {
      const body: UpdateBoard = {
        background: this.background
      }

      this.boardHttpService.update(this.board.id, body).subscribe();
    }
  }
}
