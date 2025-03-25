import {Component, Input} from '@angular/core';
import {Card, UpdateCard} from '../../../core/models/card';
import {CardHttpService} from '../../../core/services/http/card-http.service';
import {NgIf} from '@angular/common';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {CardCommunicationService} from '../../../core/services/communication/card-communication.service';

@Component({
  selector: 'app-detail-description',
  imports: [
    NgIf,
    ReactiveFormsModule
  ],
  templateUrl: './detail-description.component.html',
  styleUrl: './detail-description.component.css'
})
export class DetailDescriptionComponent {
  @Input() card: Card | undefined = undefined;
  isDescriptionActive: boolean = false;

  formUpdateCardDescription: FormGroup = new FormGroup({
    description: new FormControl('', [Validators.maxLength(255)]),
  });

  constructor(private cardHttpService: CardHttpService,
              private cardCommunicationService: CardCommunicationService) {}

  ngOnInit(): void {
    if (this.card) {
      this.formUpdateCardDescription.patchValue({
        description: this.card.description
      });
      this.isDescriptionActive = this.card.description.length > 0;
    }
    this.cardCommunicationService.updateCard$.subscribe((card: Card | null) => {
      if (card != null) {
        this.formUpdateCardDescription.patchValue({
          description: card.description
        });
        this.isDescriptionActive = card.description.length > 0;
      }
    })
  }

  onSubmitUpdateCardDescription(): void {
    if (this.formUpdateCardDescription.invalid) {
      this.formUpdateCardDescription.markAllAsTouched();
      return;
    }
    if (this.card !== undefined) {
      const body: UpdateCard = {
        description: this.formUpdateCardDescription.value.description
      }

      this.cardHttpService.update(this.card.id, body).subscribe();
    }
  }
}
