import {Component, Input} from '@angular/core';
import {CardListComponent} from '../card-list/card-list.component';
import {List} from '../../../core/models/list';
import {NgIf} from '@angular/common';
import {CardCreateFormComponent} from '../card-create-form/card-create-form.component';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {BtnDeleteListComponent} from '../btn-delete-list/btn-delete-list.component';
import {ListUpdateTitleFormComponent} from '../list-update-title-form/list-update-title-form.component';

@Component({
  selector: 'app-list-item',
  imports: [
    CardListComponent,
    NgIf,
    CardCreateFormComponent,
    FormsModule,
    ReactiveFormsModule,
    BtnDeleteListComponent,
    ListUpdateTitleFormComponent
  ],
  templateUrl: './list-item.component.html',
  styleUrl: './list-item.component.css'
})
export class ListItemComponent {
  @Input() lists: List[] = [];
  @Input() list: List | undefined = undefined;
  isCreateCardFormOpen: boolean = false;
  cardsCount: number = 0;

  constructor() {
  }


  openCreateCardForm(list: List): void {
    this.isCreateCardFormOpen = true;
    this.list = list;
    this.cardsCount = list?.cards?.length ?? 0;
  }

  closeCreateCardForm(): void {
    this.isCreateCardFormOpen = false;
  }
}
