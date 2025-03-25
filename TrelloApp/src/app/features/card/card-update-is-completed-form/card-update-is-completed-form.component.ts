import {Component, Input} from '@angular/core';
import {Card, UpdateCard} from '../../../core/models/card';
import {CardHttpService} from '../../../core/services/http/card-http.service';
import {NgIf} from '@angular/common';
import {CheckboxComponent} from '../../../shared/components/checkbox/checkbox.component';

@Component({
  selector: 'app-card-update-is-completed-form',
  imports: [
    NgIf,
    CheckboxComponent
  ],
  templateUrl: './card-update-is-completed-form.component.html',
  styleUrl: './card-update-is-completed-form.component.css'
})
export class CardUpdateIsCompletedFormComponent {
  @Input() card: Card | undefined = undefined;

  constructor(private cardHttpService: CardHttpService) {
  }

  onSubmitUpdateIsCompleted(): void {
    if (this.card !== undefined) {
      const body: UpdateCard = {
        isCompleted: !this.card.isCompleted
      }

      this.cardHttpService.update(this.card.id, body).subscribe();
    }
  }
}
