import {Component, EventEmitter, Output} from '@angular/core';
import {BtnComponent} from "../../../shared/components/btn/btn.component";
import {InputComponent} from "../../../shared/components/input/input.component";
import {NgForOf} from "@angular/common";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {ResourcesService} from '../../../core/services/resources.service';
import {BoardHttpService} from '../../../core/services/http/board-http.service';
import {AddBoard, Board} from '../../../core/models/board';
import {BoardCommunicationService} from '../../../core/services/communication/board-communication.service';

@Component({
  selector: 'app-board-create-form',
    imports: [
        BtnComponent,
        InputComponent,
        NgForOf,
        ReactiveFormsModule
    ],
  templateUrl: './board-create-form.component.html',
  styleUrl: './board-create-form.component.css'
})
export class BoardCreateFormComponent {
  @Output() close: EventEmitter<void> = new EventEmitter<void>();
  boardBackgroundPath: string = "";
  boardBackgrounds: string[] = [];
  selectedBackground: number = 0;

  formCreateBoard: FormGroup = new FormGroup({
    title: new FormControl('', [Validators.required, Validators.maxLength(32)]),
    background: new FormControl('', [Validators.required])
  });

  errorMessages: any = {
    title: {
      required: 'Email is required.',
      maxlength: 'The email must be less than 32 characters.'
    }
  };

  constructor(private resourcesService: ResourcesService,
              private boardHttpService: BoardHttpService,
              private boardCommunicationService: BoardCommunicationService) {}

  ngOnInit(): void {
    this.boardBackgrounds = this.resourcesService.boardBackgrounds;
    this.boardBackgroundPath = this.resourcesService.boardBackgroundPath;
    this.formCreateBoard.patchValue({
      background: this.boardBackgrounds[this.selectedBackground],
    });
  }

  getErrorMessage(controlName: string): string {
    const control = this.formCreateBoard.get(controlName);
    if (control?.errors) {
      for (const error in control.errors) {
        if (this.errorMessages[controlName][error]) {
          return this.errorMessages[controlName][error];
        }
      }
    }
    return '';
  }

  selectBackground(index: number): void {
    this.selectedBackground = index;
    this.formCreateBoard.patchValue({ background: this.boardBackgrounds[index] });
  }

  onSubmitCreateBoard(): void {
    if (this.formCreateBoard.invalid) {
      this.formCreateBoard.markAllAsTouched();
      return;
    }

    const body: AddBoard = {
      title: this.formCreateBoard.value.title,
      background: this.formCreateBoard.value.background,
    }

    this.boardHttpService.add(body).subscribe({
      next: (result: Board): void => {
        this.boardCommunicationService.setAddBoard(result);
        this.onClose();
      }
    });
  }

  onClose(): void {
    this.close.emit();
  }
}
