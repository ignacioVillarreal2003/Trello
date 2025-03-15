import {Component, Input} from '@angular/core';
import {NgForOf, NgIf} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {CheckboxComponent} from '../../../shared/components/checkbox/checkbox.component';
import {Card} from '../../../core/models/card';
import {Label, LabelWithAssignment} from '../../../core/models/label';
import {SessionService} from '../../../core/services/session/session.service';
import {AlertService} from '../../../core/services/alert.service';
import {LabelHttpService} from '../../../core/services/http/label-http.service';
import {UserBoardHttpService} from '../../../core/services/http/user-board-http.service';
import {UserWithAssignment} from '../../../core/models/user';
import {AddUserCard, UserCard} from '../../../core/models/user-card';
import {CardLabelHttpService} from '../../../core/services/http/card-label-http.service';
import {AddCardLabel, CardLabel} from '../../../core/models/card-label';

@Component({
  selector: 'app-card-labels',
  imports: [
    NgForOf,
    NgIf,
    ReactiveFormsModule,
    FormsModule,
    CheckboxComponent
  ],
  templateUrl: './card-labels.component.html',
  styleUrl: './card-labels.component.css'
})
export class CardLabelsComponent {
  @Input() card: Card | undefined = undefined;
  boardId: number | undefined = undefined;
  searchTerm = '';
  labels: LabelWithAssignment[] = [];

  constructor(private sessionService: SessionService,
              private alertService: AlertService,
              private cardLabelHttpService: CardLabelHttpService,
              private labelHttpService: LabelHttpService) {
  }

  ngOnInit(): void {
    this.boardId = this.sessionService.getBoardData()?.boardId;
    this.getLabels();
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
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message);
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
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message);
        }
      });
    }
  }

  updateAssignment(label: LabelWithAssignment) {
    if (this.card !== undefined) {
      if (!label.isAssigned) {
        const body: AddCardLabel = {
          labelId: label.id
        }
        this.cardLabelHttpService.addLabelToCard(this.card.id, body).subscribe({
          next: (result: CardLabel): void => {
            label.isAssigned = true;
          },
          error: (error: Error): void => {
            this.alertService.ErrorMessage(error.message);
          }
        })
      } else {
        this.cardLabelHttpService.removeLabelFromCard(this.card.id, label.id).subscribe({
          next: (result: void): void => {
            label.isAssigned = false;
          },
          error: (error: Error): void => {
            this.alertService.ErrorMessage(error.message);
          }
        })
      }
    }
  }
}
