import {Component, Input} from '@angular/core';
import {CheckboxComponent} from "../../../shared/components/checkbox/checkbox.component";
import {Label, LabelWithAssignment} from '../../../core/models/label';
import {AddCardLabel, CardLabel} from '../../../core/models/card-label';
import {SessionService} from '../../../core/services/session/session.service';
import {AlertService} from '../../../core/services/alert.service';
import {CardLabelHttpService} from '../../../core/services/http/card-label-http.service';
import {LabelHttpService} from '../../../core/services/http/label-http.service';
import {Card} from '../../../core/models/card';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-label-item',
  imports: [
    CheckboxComponent,
    NgIf
  ],
  templateUrl: './label-item.component.html',
  styleUrl: './label-item.component.css'
})
export class LabelItemComponent {
  @Input() card: Card | undefined = undefined;
  @Input() label: LabelWithAssignment | undefined = undefined;

  constructor(private cardLabelHttpService: CardLabelHttpService) {
  }

  updateAssignment(label: LabelWithAssignment): void {
    if (this.card !== undefined) {
      if (!label.isAssigned) {
        const body: AddCardLabel = {
          labelId: label.id
        }

        this.cardLabelHttpService.addLabelToCard(this.card.id, body).subscribe();
      } else {
        this.cardLabelHttpService.removeLabelFromCard(this.card.id, label.id).subscribe();
      }
    }
  }
}
