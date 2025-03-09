import {Component, EventEmitter, Input, Output} from '@angular/core';
import {AlertService} from '../../../core/services/alert.service';
import {ListHttpService} from '../../../core/services/http/list-http.service';
import {CommunicationService} from '../../../core/services/communication.service';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {InputComponent} from '../../../shared/components/input/input.component';
import {BtnComponent} from '../../../shared/components/btn/btn.component';

@Component({
  selector: 'app-create-list-modal',
  imports: [
    ReactiveFormsModule,
    InputComponent,
    BtnComponent
  ],
  templateUrl: './create-list-modal.component.html',
  standalone: true,
  styleUrl: './create-list-modal.component.css'
})
export class CreateListModalComponent {

  constructor(private alertService: AlertService,
              private listHttpService: ListHttpService,
              private communicationService: CommunicationService) {}

  /* Board */
  @Input() boardId: number | undefined = undefined;

  /* Lists */
  @Input() listsCount: number | undefined = undefined;

  /* Create list */
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
    if (this.boardId != undefined && this.listsCount != undefined) {
      this.listHttpService.postList(this.createListForm.value.title, this.listsCount, this.boardId).subscribe({
        next: (response: any): void => {
          this.alertService.SuccessMessage('Successfully created list.');
          this.communicationService.triggerRefreshLists();
        },
        error: (error: any): void => {
          const errorMessage: string = error?.message || 'Error in the server. Try again later.';
          this.alertService.ErrorMessage(errorMessage);
        }
      });
    }
  }

  /* Close */
  @Output() close: EventEmitter<void> = new EventEmitter<void>();

  onClose(): void {
    this.close.emit();
  }
}
