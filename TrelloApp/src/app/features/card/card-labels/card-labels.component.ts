import {Component, Input} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {LabelListComponent} from '../label-list/label-list.component';
import {Card} from '../../../core/models/card';
import {InputComponent} from '../../../shared/components/input/input.component';

@Component({
  selector: 'app-card-labels',
  imports: [
    ReactiveFormsModule,
    LabelListComponent,
    FormsModule,
    InputComponent
  ],
  templateUrl: './card-labels.component.html',
  styleUrl: './card-labels.component.css'
})
export class CardLabelsComponent {
  @Input() card: Card | undefined = undefined;
  @Input() boardId: number | undefined = undefined;
  searchTerm = '';
}
