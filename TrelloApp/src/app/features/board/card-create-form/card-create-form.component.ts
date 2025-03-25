import {Component, EventEmitter, Input, Output} from '@angular/core';
import {BtnComponent} from "../../../shared/components/btn/btn.component";
import {InputComponent} from "../../../shared/components/input/input.component";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {List} from '../../../core/models/list';
import {CardHttpService} from '../../../core/services/http/card-http.service';
import {AddCard, Card} from '../../../core/models/card';
import {CardCommunicationService} from '../../../core/services/communication/card-communication.service';

@Component({
  selector: 'app-card-create-form',
    imports: [
        BtnComponent,
        InputComponent,
        ReactiveFormsModule
    ],
  templateUrl: './card-create-form.component.html',
  styleUrl: './card-create-form.component.css'
})
export class CardCreateFormComponent {
  @Input() list: List | undefined = undefined;
  @Output() close: EventEmitter<void> = new EventEmitter<void>();
  @Input() cardsCount: number = 0;

  formCreateCard: FormGroup = new FormGroup({
    title: new FormControl('', [Validators.required, Validators.maxLength(64)]),
    description: new FormControl('', [Validators.required, Validators.maxLength(255)])
  });

  errorMessages: any = {
    title: {
      required: 'Title is required.',
      maxlength: 'Title must be less than 64 characters.'
    },
    description: {
      required: 'Title is required.',
      maxlength: 'Title must be less than 255 characters.'
    }
  };

  constructor(private cardHttpService: CardHttpService) {}

  getErrorMessage(controlName: string): string {
    const control = this.formCreateCard.get(controlName);
    if (control?.errors) {
      for (const error in control.errors) {
        if (this.errorMessages[controlName][error]) {
          return this.errorMessages[controlName][error];
        }
      }
    }
    return '';
  }

  onSubmitCreateCard(): void {
    if (this.formCreateCard.invalid) {
      this.formCreateCard.markAllAsTouched();
      return;
    }

    if (this.list != undefined) {
      const body: AddCard = {
        title: this.formCreateCard.value.title,
        description: this.formCreateCard.value.description,
        position: this.cardsCount
      }
      this.cardHttpService.add(this.list.id, body).subscribe({
        next: (): void => {
          this.onClose();
        }
      });
    }
  }

  onClose(): void {
    this.close.emit();
  }
}
