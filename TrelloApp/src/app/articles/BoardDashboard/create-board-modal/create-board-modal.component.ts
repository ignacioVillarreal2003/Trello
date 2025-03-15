import {Component, EventEmitter, Output} from '@angular/core';
import {ResourcesService} from '../../../core/services/resources.service';
import {AlertService} from '../../../core/services/alert.service';
import {BoardHttpService} from '../../../core/services/http/board-http.service';
import {CommunicationService} from '../../../core/services/communication.service';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {NgForOf, NgStyle} from '@angular/common';
import {InputComponent} from '../../../shared/components/input/input.component';
import {BtnComponent} from '../../../shared/components/btn/btn.component';
import {AddBoard, UpdateBoard} from '../../../core/models/board';

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
  @Output() close: EventEmitter<void> = new EventEmitter<void>();
  boardBackgroundPath: string = "";
  boardBackgrounds: string[] = [];
  selectedBackground: number = 0;

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

  selectBackground(index: number): void {
    this.selectedBackground = index;
    this.createBoardForm.patchValue({ background: this.boardBackgrounds[index] });
  }

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
    const body: AddBoard = {
      title: this.createBoardForm.value.title,
      description: this.createBoardForm.value.description,
      background: this.createBoardForm.value.background,
    }
    this.boardHttpService.add(body).subscribe({
      next: (response: any): void => {
        this.alertService.SuccessMessage('Successfully created board.');
        this.communicationService.triggerRefreshBoards();
        this.onClose();
      },
      error: (error: Error): void => {
        this.alertService.ErrorMessage(error.message);
      }
    });
  }

  onClose(): void {
    this.close.emit();
  }
}
