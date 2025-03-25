import {Component, Input} from '@angular/core';
import {AvatarComponent} from "../../../shared/components/avatar/avatar.component";
import {NgForOf, NgIf} from "@angular/common";
import {User} from '../../../core/models/user';
import {Card} from '../../../core/models/card';
import {UserCardHttpService} from '../../../core/services/http/user-card-http.service';
import {UserCardCommunicationService} from '../../../core/services/communication/user-card-communication.service';
import {CardLabel} from '../../../core/models/card-label';
import {UserCard} from '../../../core/models/user-card';

@Component({
  selector: 'app-detail-user-list',
    imports: [
        AvatarComponent,
        NgForOf,
        NgIf
    ],
  templateUrl: './detail-user-list.component.html',
  styleUrl: './detail-user-list.component.css'
})
export class DetailUserListComponent {
  @Input() card: Card | undefined = undefined;
  users: User[] = [];

  constructor(private userCardHttpService: UserCardHttpService,
              private userCardCommunicationService: UserCardCommunicationService) {}

  ngOnInit(): void {
    this.getUsers();
    this.userCardCommunicationService.addUserCard$.subscribe((userCard: UserCard | null) => {
      if (userCard != null && this.card != undefined && this.card.id == userCard.cardId) {
        this.users.push(userCard.user);
      }
    });
    this.userCardCommunicationService.deleteUserCard$.subscribe((value: { userId: number, cardId: number } | null) => {
      if (value != null && this.card != undefined && this.card.id == value.cardId) {
        this.users = this.users.filter(u => u.id != value.userId);
      }
    });
  }

  getUsers(): void {
    if (this.card !== undefined) {
      this.userCardHttpService.getUsersByCardId(this.card.id).subscribe({
        next: (result: User[]): void => {
          this.users = result;
        }
      });
    }
  }
}
