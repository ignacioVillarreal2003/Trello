import {Component, Input} from '@angular/core';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {Board, UpdateBoard} from '../../../core/models/board';
import {BoardHttpService} from '../../../core/services/http/board-http.service';
import {BoardCommunicationService} from '../../../core/services/communication/board-communication.service';

@Component({
  selector: 'app-board-update-title-form',
    imports: [
        FormsModule,
        ReactiveFormsModule
    ],
  templateUrl: './board-update-title-form.component.html',
  styleUrl: './board-update-title-form.component.css'
})
export class BoardUpdateTitleFormComponent {
  @Input() board: Board | undefined = undefined;

  formUpdateBoardTitle: FormGroup = new FormGroup({
    title: new FormControl('', [Validators.required, Validators.maxLength(64)]),
  });

  constructor(private boardHttpService: BoardHttpService,
              private boardCommunicationService: BoardCommunicationService) {
  }

  ngOnInit(): void {
    if (this.board) {
      this.formUpdateBoardTitle.patchValue({
        title: this.board.title
      });
    }
    this.boardCommunicationService.updateBoard$.subscribe((board: Board | null): void => {
      if (board !== null) {
        this.formUpdateBoardTitle.patchValue({
          title: board.title
        });
      }
    })
  }

  onSubmitUpdateBoardTitle(): void {
    if (this.formUpdateBoardTitle.invalid) {
      this.formUpdateBoardTitle.markAllAsTouched();
      return;
    }
    if (this.board) {
      const body: UpdateBoard = {
        title: this.formUpdateBoardTitle.value.title
      }

      this.boardHttpService.update(this.board.id, body).subscribe();
    }
  }
}
