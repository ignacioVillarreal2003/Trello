import {Component, Input} from '@angular/core';
import {NgForOf, NgIf} from "@angular/common";
import {Label} from '../../../core/models/label';
import {Card} from '../../../core/models/card';
import {CardLabelHttpService} from '../../../core/services/http/card-label-http.service';
import {CardLabelCommunicationService} from '../../../core/services/communication/card-label-communication.service';
import {CardLabel} from '../../../core/models/card-label';

@Component({
  selector: 'app-detail-label-list',
    imports: [
        NgForOf,
        NgIf
    ],
  templateUrl: './detail-label-list.component.html',
  styleUrl: './detail-label-list.component.css'
})
export class DetailLabelListComponent {
  @Input() card: Card | undefined = undefined;
  labels: Label[] = [];

  constructor(private cardLabelHttpService: CardLabelHttpService,
              private cardLabelCommunicationService: CardLabelCommunicationService) {}

  ngOnInit(): void {
    this.getLabels();
    this.cardLabelCommunicationService.addCardLabel$.subscribe((cardLabel: CardLabel | null) => {
      if (cardLabel != null && this.card != undefined && this.card.id == cardLabel.cardId) {
        this.labels.push(cardLabel.label);
      }
    });
    this.cardLabelCommunicationService.deleteCardLabel$.subscribe((value: { labelId: number, cardId: number } | null) => {
      if (value != null && this.card != undefined && this.card.id == value.cardId) {
        this.labels = this.labels.filter(l => l.id != value.labelId);
      }
    });
  }

  getLabels(): void {
    if (this.card !== undefined) {
      this.cardLabelHttpService.getLabelsByCardId(this.card.id).subscribe({
        next: (result: Label[]): void => {
          this.labels = result;
        }
      });
    }
  }
}
