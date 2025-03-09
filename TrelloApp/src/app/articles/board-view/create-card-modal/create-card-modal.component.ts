import {Component, EventEmitter, Input, Output} from '@angular/core';
import {AlertService} from '../../../core/services/alert.service';
import {CardHttpService} from '../../../core/services/http/card-http.service';
import {CommunicationService} from '../../../core/services/communication.service';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {InputComponent} from '../../../shared/components/input/input.component';
import {BtnComponent} from "../../../shared/components/btn/btn.component";
import {List} from '../../../core/models/list';

@Component({
  selector: 'app-create-card-modal',
  imports: [
    ReactiveFormsModule,
    InputComponent,
    BtnComponent
  ],
  templateUrl: './create-card-modal.component.html',
  standalone: true,
  styleUrl: './create-card-modal.component.css'
})
export class CreateCardModalComponent {
  @Input() listId: number | undefined = undefined;

  constructor(private alertService: AlertService,
              private cardHttpService: CardHttpService,
              private communicationService: CommunicationService) {}

  /* Create card */
  createCardForm: FormGroup = new FormGroup({
    title: new FormControl('', [Validators.required]),
    description: new FormControl('', [Validators.required])
  });

  onSubmitCreateCard(): void {
    if (this.createCardForm.invalid) {
      if (this.createCardForm.controls["title"].errors) {
        this.alertService.ErrorMessage('Title is required.');
      }
      if (this.createCardForm.controls["description"].errors) {
        this.alertService.ErrorMessage('Description is required.');
      }
      return;
    }
    if (this.listId) {
      this.cardHttpService.postCard(this.createCardForm.value.title, this.createCardForm.value.description, this.listId).subscribe({
        next: (response: any): void => {
          this.alertService.SuccessMessage('Successfully created list.ts.');
          this.communicationService.triggerRefreshCards();
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
  @Input() list: List | undefined;

  onClose(): void {
    this.close.emit();
  }
}
