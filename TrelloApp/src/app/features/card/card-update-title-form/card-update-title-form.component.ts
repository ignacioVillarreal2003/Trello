import {Component, Input} from '@angular/core';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {UpdateBoard} from '../../../core/models/board';
import {CardHttpService} from '../../../core/services/http/card-http.service';
import {Card} from '../../../core/models/card';
import {CardCommunicationService} from '../../../core/services/communication/card-communication.service';

@Component({
  selector: 'app-card-update-title-form',
    imports: [
        FormsModule,
        ReactiveFormsModule
    ],
  templateUrl: './card-update-title-form.component.html',
  styleUrl: './card-update-title-form.component.css'
})
export class CardUpdateTitleFormComponent {
  @Input() card: Card | undefined = undefined;

  formUpdateCardTitle: FormGroup = new FormGroup({
    title: new FormControl('', [Validators.required, Validators.maxLength(64)]),
  });

  constructor(private cardHttpService: CardHttpService,
              private cardCommunicationService: CardCommunicationService) {
  }

  ngOnInit(): void {
    if (this.card) {
      this.formUpdateCardTitle.patchValue({
        title: this.card.title
      });
    }
    this.cardCommunicationService.updateCard$.subscribe((card: Card | null): void => {
      if (card != null) {
        this.formUpdateCardTitle.patchValue({
          title: card.title
        });
      }
    })
  }

  onSubmitUpdateCardTitle(): void {
    if (this.formUpdateCardTitle.invalid) {
      this.formUpdateCardTitle.markAllAsTouched();
      return;
    }
    if (this.card) {
      const body: UpdateBoard = {
        title: this.formUpdateCardTitle.value.title
      }

      this.cardHttpService.update(this.card.id, body).subscribe();
    }
  }
}
