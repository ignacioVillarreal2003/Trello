import {Component, Input} from '@angular/core';
import {NgForOf, NgIf} from "@angular/common";
import {UserItemComponent} from '../user-item/user-item.component';
import {Card} from '../../../core/models/card';
import {User, UserWithAssignment} from '../../../core/models/user';
import {SessionService} from '../../../core/services/session/session.service';
import {AlertService} from '../../../core/services/alert.service';
import {UserCardHttpService} from '../../../core/services/http/user-card-http.service';
import {UserBoardHttpService} from '../../../core/services/http/user-board-http.service';
import {UserCardCommunicationService} from '../../../core/services/communication/user-card-communication.service';
import {UserBoardCommunicationService} from '../../../core/services/communication/user-board-communication.service';
import {CardLabel} from '../../../core/models/card-label';
import {Label} from '../../../core/models/label';
import {UserCard} from '../../../core/models/user-card';
import {UserBoard} from '../../../core/models/user-board';

@Component({
  selector: 'app-user-list',
  imports: [
    NgForOf,
    NgIf,
    UserItemComponent
  ],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css'
})
export class UserListComponent {
  @Input() card: Card | undefined = undefined;
  @Input() boardId: number | undefined = undefined;
  @Input() searchTerm: string = '';
  users: UserWithAssignment[] = [];

  constructor(private userCardHttpService: UserCardHttpService,
              private userCardCommunicationService: UserCardCommunicationService,
              private userBoardCommunicationService: UserBoardCommunicationService,
              private userBoardHttpService: UserBoardHttpService) {
  }

  ngOnInit(): void {
    this.getUsers();
    this.userCardCommunicationService.addUserCard$.subscribe((userCard: UserCard | null) => {
      if (userCard != null && this.card != undefined && this.card.id == userCard.cardId) {
        this.users.map(u => u.id == userCard.userId ? u.isAssigned = true : u);
      }
    });
    this.userCardCommunicationService.deleteUserCard$.subscribe((value: { userId: number, cardId: number } | null) => {
      if (value != null && this.card != undefined && this.card.id == value.cardId) {
        this.users.map(u => u.id == value.userId ? u.isAssigned = false : u);
      }
    });
    this.userBoardCommunicationService.addUserBoard$.subscribe((userBoard: UserBoard | null): void => {
      if (userBoard !== null) {
        this.users.push({
          ...userBoard.user,
          isAssigned: false
        });
      }
    });
    this.userBoardCommunicationService.updateUserBoard$.subscribe((userBoard: UserBoard | null): void => {
      if (userBoard !== null) {
        this.users = this.users.map(u => u.id == userBoard.user.id ? {...userBoard.user,
          isAssigned: u.isAssigned
        } : u);
      }
    });
    this.userBoardCommunicationService.deleteUserBoard$.subscribe((value: { userId: number, boardId: number } | null): void => {
      if (value !== null) {
        this.users = this.users.filter(u => u.id != value.userId);
      }
    });
  }

  get filteredUsers(): UserWithAssignment[] {
    return this.users.filter((user: UserWithAssignment): boolean =>
      user.username.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  getUsers(): void {
    if (this.boardId !== undefined) {
      this.userBoardHttpService.getUsersByBoardId(this.boardId).subscribe({
        next: (result: User[]): void => {
          this.users = result.map((user: User): any => ({
            ...user,
            isAssigned: false
          }));
          this.getAssignedUsers();
        }
      })
    }
  }

  getAssignedUsers(): void {
    if (this.card !== undefined) {
      this.userCardHttpService.getUsersByCardId(this.card.id).subscribe({
        next: (result: User[]): void => {
          const assignedUserIds = new Set(result.map((user: User): any => user.id));
          this.users = this.users.map((user: User): UserWithAssignment => ({
            ...user,
            isAssigned: assignedUserIds.has(user.id)
          }));
        }
      })
    }
  }
}
