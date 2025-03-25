import {Component, Input} from '@angular/core';
import {CheckboxComponent} from "../../../shared/components/checkbox/checkbox.component";
import {UserWithAssignment} from '../../../core/models/user';
import {NgIf} from '@angular/common';
import {AddUserCard} from '../../../core/models/user-card';
import {Card} from '../../../core/models/card';
import {UserCardHttpService} from '../../../core/services/http/user-card-http.service';

@Component({
  selector: 'app-user-item',
  imports: [
    CheckboxComponent,
    NgIf
  ],
  templateUrl: './user-item.component.html',
  styleUrl: './user-item.component.css'
})
export class UserItemComponent {
  @Input() card: Card | undefined = undefined;
  @Input() user: UserWithAssignment | undefined = undefined;

  constructor(private userCardHttpService: UserCardHttpService) {}

  updateAssignment() {
    if (this.card !== undefined && this.user !== undefined) {
      if (!this.user.isAssigned) {
        const body: AddUserCard = {
          userId: this.user.id
        }
        this.userCardHttpService.addUserToCard(this.card.id, body).subscribe();
      } else {
        this.userCardHttpService.removeUserFromCard(this.user.id, this.card.id).subscribe()
      }
    }
  }
}
