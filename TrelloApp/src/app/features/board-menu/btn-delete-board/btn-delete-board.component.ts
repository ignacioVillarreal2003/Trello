import {Component, Input} from '@angular/core';
import {BtnComponent} from '../../../shared/components/btn/btn.component';
import {Board} from '../../../core/models/board';
import {BoardHttpService} from '../../../core/services/http/board-http.service';
import {Router} from '@angular/router';
import {BoardCommunicationService} from '../../../core/services/communication/board-communication.service';

@Component({
  selector: 'app-btn-delete-board',
  imports: [
    BtnComponent
  ],
  templateUrl: './btn-delete-board.component.html',
  styleUrl: './btn-delete-board.component.css'
})
export class BtnDeleteBoardComponent {
  @Input() board: Board | undefined = undefined;

  constructor(private boardHttpService: BoardHttpService,
              private boardCommunicationService: BoardCommunicationService,
              private router: Router) {
  }

  onDelete(): void {
    if (this.board !== undefined) {
      this.boardHttpService.delete(this.board.id).subscribe({
        next: (): void => {
          this.router.navigate(['dashboard']);
          if (this.board != undefined) {
            this.boardCommunicationService.setDeleteBoard(this.board.id);
          }
        }
      })
    }
  }
}
