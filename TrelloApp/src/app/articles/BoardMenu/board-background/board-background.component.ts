import {Component, Input} from '@angular/core';
import {NgForOf} from "@angular/common";
import {ResourcesService} from '../../../core/services/resources.service';
import {BoardHttpService} from '../../../core/services/http/board-http.service';
import {Board, UpdateBoard} from '../../../core/models/board';
import {AlertService} from '../../../core/services/alert.service';
import {AddUserBoard} from '../../../core/models/user-board';

@Component({
  selector: 'app-board-background',
    imports: [
        NgForOf
    ],
  templateUrl: './board-background.component.html',
  styleUrl: './board-background.component.css'
})
export class BoardBackgroundComponent {
  @Input() board: Board | undefined = undefined;
  boardBackgrounds: string[] = [];
  boardBackgroundPath: string | undefined = undefined;
  selectedBackground: number = 0;

  constructor(private resourcesService: ResourcesService,
              private boardHttpService: BoardHttpService,
              private alertService: AlertService) {}

  ngOnInit(): void {
    this.boardBackgrounds = this.resourcesService.boardBackgrounds;
    this.boardBackgroundPath = this.resourcesService.boardBackgroundPath;
    const actualBg: string | undefined = this.board?.background;
    if (actualBg !== undefined) {
      this.selectedBackground = this.boardBackgrounds.indexOf(actualBg);
    }
  }

  selectBackground(index: number) {
    this.selectedBackground = index;
    if (this.board) {
      const body: UpdateBoard = {
       background: this.boardBackgrounds[this.selectedBackground]
      }
      this.boardHttpService.update(this.board.id, body).subscribe({
        next: (result: Board): void => {
          this.alertService.SuccessMessage('Successfully updated board.');
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message)
        }
      });
    }
  }
}
