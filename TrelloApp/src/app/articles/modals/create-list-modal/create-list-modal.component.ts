import {Component, EventEmitter, Input, Output} from '@angular/core';
import {AlertService} from '../../../core/services/alert.service';
import {ListHttpService} from '../../../core/services/http/list-http.service';
import {CommunicationService} from '../../../core/services/communication.service';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';

@Component({
  selector: 'app-create-list-modal',
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './create-list-modal.component.html',
  standalone: true,
  styleUrl: './create-list-modal.component.css'
})
export class CreateListModalComponent {
  @Input() boardId: number | undefined = undefined;
  @Output() close: EventEmitter<void> = new EventEmitter<void>();

  constructor(private alertService: AlertService,
              private listHttpService: ListHttpService,
              private communicationService: CommunicationService) {}

  createListForm: FormGroup = new FormGroup({
    title: new FormControl('', [Validators.required]),
  });

  onSubmitCreateList(): void {
    if (this.createListForm.invalid) {
      if (this.createListForm.controls["title"].errors) {
        this.alertService.ErrorMessage('Title is required.');
      }
      return;
    }
    if (this.boardId) {
      this.listHttpService.postList(this.createListForm.value.title, this.boardId).subscribe({
        next: (response: any): void => {
          this.alertService.SuccessMessage('Successfully created list.ts.');
          this.communicationService.triggerRefreshLists();
        },
        error: (error: any): void => {
          const errorMessage: string = error?.message || 'Error in the server. Try again later.';
          this.alertService.ErrorMessage(errorMessage);
        }
      });
    }
  }

  onClose(): void {
    this.close.emit();
  }
}
