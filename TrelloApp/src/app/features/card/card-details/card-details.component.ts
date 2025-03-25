import {Component, Input} from '@angular/core';
import {Card} from '../../../core/models/card';
import {NgIf} from '@angular/common';
import {DetailLabelListComponent} from '../detail-label-list/detail-label-list.component';
import {DetailUserListComponent} from '../detail-user-list/detail-user-list.component';
import {DetailDescriptionComponent} from '../detail-description/detail-description.component';

@Component({
  selector: 'app-card-details',
  imports: [
    NgIf,
    DetailLabelListComponent,
    DetailUserListComponent,
    DetailDescriptionComponent
  ],
  templateUrl: './card-details.component.html',
  styleUrl: './card-details.component.css'
})
export class CardDetailsComponent {
  @Input() card: Card | undefined = undefined;
}
