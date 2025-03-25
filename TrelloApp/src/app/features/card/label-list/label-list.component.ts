import {Component, Input} from '@angular/core';
import {NgForOf, NgIf} from "@angular/common";
import {LabelItemComponent} from '../label-item/label-item.component';
import {Card} from '../../../core/models/card';
import {Label, LabelWithAssignment} from '../../../core/models/label';
import {CardLabelHttpService} from '../../../core/services/http/card-label-http.service';
import {LabelHttpService} from '../../../core/services/http/label-http.service';
import {CardLabelCommunicationService} from '../../../core/services/communication/card-label-communication.service';
import {CardLabel} from '../../../core/models/card-label';
import {LabelCommunicationService} from '../../../core/services/communication/label-communication.service';

@Component({
  selector: 'app-label-list',
  imports: [
    NgForOf,
    NgIf,
    LabelItemComponent
  ],
  templateUrl: './label-list.component.html',
  styleUrl: './label-list.component.css'
})
export class LabelListComponent {
  @Input() card: Card | undefined = undefined;
  @Input() boardId: number | undefined = undefined;
  @Input() searchTerm = '';
  labels: LabelWithAssignment[] = [];

  constructor(private cardLabelHttpService: CardLabelHttpService,
              private labelHttpService: LabelHttpService,
              private cardLabelCommunicationService: CardLabelCommunicationService,
              private labelCommunicationService: LabelCommunicationService) {
  }

  ngOnInit(): void {
    this.getLabels();
    this.cardLabelCommunicationService.addCardLabel$.subscribe((cardLabel: CardLabel | null) => {
      if (cardLabel != null && this.card != undefined && this.card.id == cardLabel.cardId) {
        this.labels.map(l => l.id == cardLabel.labelId ? l.isAssigned = true : l);
      }
    });
    this.cardLabelCommunicationService.deleteCardLabel$.subscribe((value: { labelId: number, cardId: number } | null) => {
      if (value != null && this.card != undefined && this.card.id == value.cardId) {
        this.labels.map(l => l.id == value.labelId ? l.isAssigned = false : l);
      }
    });
    this.labelCommunicationService.addLabel$.subscribe((label: Label | null): void => {
      if (label !== null) {
        this.labels.push({
          ...label,
          isAssigned: false
        });
      }
    });
    this.labelCommunicationService.updateLabel$.subscribe((label: Label | null): void => {
      if (label !== null) {
        this.labels = this.labels.map(l => l.id == label.id ? {...label,
          isAssigned: l.isAssigned
        } : l);
      }
    });
    this.labelCommunicationService.deleteLabel$.subscribe((labelId: number | null): void => {
      if (labelId !== null) {
        this.labels = this.labels.filter(l => l.id != labelId);
      }
    });
  }

  get filteredLabels(): LabelWithAssignment[] {
    return this.labels.filter((label: LabelWithAssignment): any =>
      label.title.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  getLabels(): void {
    if (this.boardId !== undefined) {
      this.labelHttpService.getLabelsByBoardId(this.boardId).subscribe({
        next: (result: Label[]): void => {
          this.labels = result.map((label: Label): any => ({
            ...label,
            isAssigned: false
          }));
          this.getAssignedLabels();
        }
      });
    }
  }

  getAssignedLabels(): void {
    if (this.card !== undefined) {
      this.cardLabelHttpService.getLabelsByCardId(this.card.id).subscribe({
        next: (assignedLabels: Label[]): void => {
          const assignedLabelIds = new Set(assignedLabels.map(label => label.id));
          this.labels = this.labels.map(label => ({
            ...label,
            isAssigned: assignedLabelIds.has(label.id)
          }));
        }
      });
    }
  }
}
