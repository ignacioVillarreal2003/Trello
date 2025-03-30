import {Component, Input} from '@angular/core';
import {Board} from '../../../core/models/board';
import {Label} from '../../../core/models/label';
import {LabelHttpService} from '../../../core/services/http/label-http.service';
import {NgForOf} from '@angular/common';
import {LabelItemComponent} from '../label-item/label-item.component';
import {LabelCommunicationService} from '../../../core/services/communication/label-communication.service';

@Component({
  selector: 'app-label-list',
  imports: [
    NgForOf,
    LabelItemComponent,
  ],
  templateUrl: './label-list.component.html',
  styleUrl: './label-list.component.css'
})
export class LabelListComponent {
  @Input() board: Board | undefined = undefined;
  labels: Label[] = [];

  constructor(private labelHttpService: LabelHttpService,
              private labelCommunicationService: LabelCommunicationService) { }

  ngOnInit(): void {
    this.getLabels();
    this.labelCommunicationService.addLabel$.subscribe((label: Label | null): void => {
      if (label !== null) {
        this.labels.push(label);
      }
    });
    this.labelCommunicationService.updateLabel$.subscribe((label: Label | null): void => {
      if (label !== null) {
        this.labels = this.labels.map(l => l.id == label.id ? label : l);
      }
    });
    this.labelCommunicationService.deleteLabel$.subscribe((labelId: number | null): void => {
      console.log(labelId)
      if (labelId !== null) {
        this.labels = this.labels.filter(l => l.id != labelId);
      }
    });
  }

  getLabels(): void {
    if (this.board) {
      this.labelHttpService.getLabelsByBoardId(this.board.id).subscribe({
        next: (result: Label[]): void => {
          this.labels = result;
        }
      })
    }
  }
}
