import {Component, EventEmitter, Input, Output} from '@angular/core';
import {AlertService} from '../../../core/services/alert.service';
import {CardHttpService} from '../../../core/services/http/card-http.service';
import {CommunicationService} from '../../../core/services/communication.service';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {InputComponent} from '../../../shared/components/input/input.component';
import {BtnComponent} from "../../../shared/components/btn/btn.component";
import {List} from '../../../core/models/list';
import {AddCard} from '../../../core/models/card';

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
  @Input() list: List | undefined = undefined;
  @Output() close: EventEmitter<void> = new EventEmitter<void>();
  @Input() cardsCount: number = 0;

  constructor(private alertService: AlertService,
              private cardHttpService: CardHttpService,
              private communicationService: CommunicationService) {}

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
    if (this.list != undefined) {
      const body: AddCard = {
        title: this.createCardForm.value.title,
        description: this.createCardForm.value.description,
        position: this.cardsCount
      }
      this.cardHttpService.add(this.list.id, body).subscribe({
        next: (response: any): void => {
          this.alertService.SuccessMessage('Successfully created card');
          this.communicationService.triggerRefreshCards();
          this.onClose();
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message);
        }
      });
    }
  }

  onClose(): void {
    this.close.emit();
  }
}
