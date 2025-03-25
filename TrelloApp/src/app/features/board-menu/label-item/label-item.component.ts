import {Component, Input} from '@angular/core';
import {Label} from '../../../core/models/label';
import {BtnDeleteComponent} from '../../../shared/components/btn-delete/btn-delete.component';
import {NgIf} from '@angular/common';
import {LabelHttpService} from '../../../core/services/http/label-http.service';

@Component({
  selector: 'app-label-item',
  imports: [
    BtnDeleteComponent,
    NgIf
  ],
  templateUrl: './label-item.component.html',
  styleUrl: './label-item.component.css'
})
export class LabelItemComponent {
  @Input() label: Label | undefined = undefined;

  constructor(private labelHttpService: LabelHttpService) { }

  deleteLabel(): void {
    if (this.label !== undefined) {
      this.labelHttpService.delete(this.label.id).subscribe();
    }
  }
}
