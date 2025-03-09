import {Component, EventEmitter, Output} from '@angular/core';
import {ResourcesService} from '../../../core/services/resources/resources.service';
import {AlertService} from '../../../core/services/alert.service';
import {BoardHttpService} from '../../../core/services/http/board-http.service';
import {CommunicationService} from '../../../core/services/communication.service';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {NgForOf, NgStyle} from '@angular/common';
import {InputComponent} from '../../../shared/components/input/input.component';
import {BtnComponent} from '../../../shared/components/btn/btn.component';

@Component({
  selector: 'app-create-board-modal',
  imports: [
    NgStyle,
    NgForOf,
    ReactiveFormsModule,
    InputComponent,
    BtnComponent
  ],
  templateUrl: './create-board-modal.component.html',
  standalone: true,
  styleUrl: './create-board-modal.component.css'
})
export class CreateBoardModalComponent {

  constructor(private resourcesService: ResourcesService,
              private alertService: AlertService,
              private boardHttpService: BoardHttpService,
              private communicationService: CommunicationService) {}

  ngOnInit(): void {
    this.boardBackgrounds = this.resourcesService.boardBackgrounds;
    this.boardBackgroundPath = this.resourcesService.boardBackgroundPath;
    this.createBoardForm.patchValue({
      background: this.boardBackgrounds[this.selectedBackground],
    });
  }

  /* Board background */
  boardBackgroundPath: string = "";
  boardBackgrounds: string[] = [];
  selectedBackground: number = 0;

  selectBackground(index: number): void {
    this.selectedBackground = index;
    this.createBoardForm.patchValue({ background: this.boardBackgrounds[index] });
  }

  /* Create board */
  createBoardForm: FormGroup = new FormGroup({
    title: new FormControl('', [Validators.required]),
    description: new FormControl(''),
    background: new FormControl('', [Validators.required])
  });

  onSubmitCreateBoard(): void {
    if (this.createBoardForm.invalid) {
      if (this.createBoardForm.controls["title"].errors) {
        this.alertService.ErrorMessage('Title is required.');
      } else if (this.createBoardForm.controls["background"].errors) {
        this.alertService.ErrorMessage('Background is required.');
      }
      return;
    }
    this.boardHttpService.addBoard(this.createBoardForm.value.title, this.createBoardForm.value.description, this.createBoardForm.value.background).subscribe({
      next: (response: any): void => {
        this.alertService.SuccessMessage('Successfully created board.');
        this.communicationService.triggerRefreshBoards();
        this.onClose();
      },
      error: (error: any): void => {
        const errorMessage: string = error?.message || 'Error in the server. Try again later.';
        this.alertService.ErrorMessage(errorMessage);      }
    });
  }

  /* Close */
  @Output() close: EventEmitter<void> = new EventEmitter<void>();

  onClose(): void {
    this.close.emit();
  }
}
