import {Component, EventEmitter, Output} from '@angular/core';
import {ResourcesService} from '../../../core/services/resources.service';
import {AlertService} from '../../../core/services/alert.service';
import {BoardHttpService} from '../../../core/services/http/board-http.service';
import {CommunicationService} from '../../../core/services/communication.service';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {NgForOf, NgStyle} from '@angular/common';

@Component({
  selector: 'app-create-board-modal',
  imports: [
    NgStyle,
    NgForOf,
    ReactiveFormsModule
  ],
  templateUrl: './create-board-modal.component.html',
  standalone: true,
  styleUrl: './create-board-modal.component.css'
})
export class CreateBoardModalComponent {
  colors: string[] = [];
  iconPath: string | undefined = undefined
  icons: string[] = [];
  selectedColor: number = 0;
  selectedIcon: number = 0;
  @Output() close: EventEmitter<void> = new EventEmitter<void>();

  constructor(private resourcesService: ResourcesService,
              private alertService: AlertService,
              private boardHttpService: BoardHttpService,
              private communicationService: CommunicationService) {}

  ngOnInit(): void {
    this.colors = this.resourcesService.colors;
    this.icons = this.resourcesService.icons;
    this.iconPath = this.resourcesService.iconPath;
    this.createBoardForm.patchValue({
      theme: this.colors[this.selectedColor],
      icon: this.icons[this.selectedIcon]
    });
  }

  selectColor(index: number): void {
    this.selectedColor = index;
    this.createBoardForm.patchValue({ theme: this.colors[index] });
  }

  selectIcon(index: number): void {
    this.selectedIcon = index;
    this.createBoardForm.patchValue({ icon: this.icons[index] });
  }

  createBoardForm: FormGroup = new FormGroup({
    title: new FormControl('', [Validators.required]),
    theme: new FormControl('', [Validators.required]),
    icon: new FormControl('', [Validators.required])
  });

  onSubmitCreateBoard(): void {
    if (this.createBoardForm.invalid) {
      if (this.createBoardForm.controls["title"].errors) {
        this.alertService.ErrorMessage('Title is required.');
      } else if (this.createBoardForm.controls["theme"].errors) {
        this.alertService.ErrorMessage('Theme is required.');
      } else if (this.createBoardForm.controls["icon"].errors) {
        this.alertService.ErrorMessage('Icon is required.');
      }
      return;
    }
    this.boardHttpService.postBoard(this.createBoardForm.value.title, this.createBoardForm.value.theme, this.createBoardForm.value.icon).subscribe({
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

  onClose(): void {
    this.close.emit();
  }
}
