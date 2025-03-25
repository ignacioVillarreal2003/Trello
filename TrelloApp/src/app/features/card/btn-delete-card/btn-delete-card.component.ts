import {Component, Input} from '@angular/core';
import {Card} from '../../../core/models/card';
import {CardHttpService} from '../../../core/services/http/card-http.service';
import {Location} from '@angular/common';

@Component({
  selector: 'app-btn-delete-card',
  imports: [],
  templateUrl: './btn-delete-card.component.html',
  styleUrl: './btn-delete-card.component.css'
})
export class BtnDeleteCardComponent {
  @Input() card: Card | undefined = undefined;

  constructor(private cardHttpService: CardHttpService,
              private location: Location) {
  }

  deleteCard(): void {
    if (this.card !== undefined) {
      this.cardHttpService.delete(this.card.id).subscribe({
        next: (): void => {
          this.goBack();
        }
      })
    }
  }

  goBack(): void {
    this.location.back();
  }
}
