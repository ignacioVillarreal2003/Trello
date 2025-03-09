import {Component, EventEmitter, Input, Output} from '@angular/core';
import {ReactiveFormsModule} from '@angular/forms';
import {NgSwitch, NgSwitchCase} from '@angular/common';
import {Card} from '../../core/models/card';
import {CardCommentsComponent} from './card-comments/card-comments.component';
import {CardLabelsComponent} from './card-labels/card-labels.component';
import {CardUsersComponent} from './card-users/card-users.component';
import {CardDetailsComponent} from './card-details/card-details.component';
import {HeaderComponent} from '../header/header.component';
import {BtnCloseComponent} from '../../shared/components/btn-close/btn-close.component';
import {CheckboxComponent} from '../../shared/components/checkbox/checkbox.component';

@Component({
  selector: 'app-card-view',
  imports: [
    ReactiveFormsModule,
    NgSwitchCase,
    NgSwitch,
    CardCommentsComponent,
    CardLabelsComponent,
    CardUsersComponent,
    CardDetailsComponent,
    HeaderComponent,
    BtnCloseComponent,
    CheckboxComponent
  ],
  templateUrl: './card-view.component.html',
  standalone: true,
  styleUrl: './card-view.component.css'
})
export class CardViewComponent {

  constructor() {}

  ngOnInit(): void {
  }

  /* Mode */
  currentMode: string = 'details';

  setMode(mode: string): void {
    this.currentMode = mode;
  }

  /* Close */
  @Output() close: EventEmitter<void> = new EventEmitter();

  onClose(): void {
    this.close.emit();
  }

  /* Card */
  cardId: string | undefined = '';
  @Input() card: Card | undefined = undefined;
}
