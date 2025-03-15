import {Component, EventEmitter, Input, Output} from '@angular/core';
import {NgForOf, NgIf} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {Card} from '../../../core/models/card';
import {User, UserWithAssignment} from '../../../core/models/user';
import {SessionService} from '../../../core/services/session/session.service';
import {Label} from '../../../core/models/label';
import {LabelHttpService} from '../../../core/services/http/label-http.service';
import {UserBoardHttpService} from '../../../core/services/http/user-board-http.service';
import {AlertService} from '../../../core/services/alert.service';
import {UserCardHttpService} from '../../../core/services/http/user-card-http.service';
import {AddUserCard, UserCard} from '../../../core/models/user-card';
import {CheckboxComponent} from '../../../shared/components/checkbox/checkbox.component';

@Component({
  selector: 'app-card-users',
  imports: [
    NgForOf,
    NgIf,
    ReactiveFormsModule,
    FormsModule,
    CheckboxComponent
  ],
  templateUrl: './card-users.component.html',
  styleUrl: './card-users.component.css'
})
export class CardUsersComponent {
  @Input() card: Card | undefined = undefined;
  boardId: number | undefined = undefined;
  searchTerm: string = '';
  users: UserWithAssignment[] = [];

  constructor(private sessionService: SessionService,
              private alertService: AlertService,
              private userCardHttpService: UserCardHttpService,
              private userBoardHttpService: UserBoardHttpService) {
  }

  ngOnInit(): void {
    this.boardId = this.sessionService.getBoardData()?.boardId;
    this.getUsers();
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
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message)
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
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message);
        }
      })
    }
  }

  updateAssignment(user: UserWithAssignment) {
    if (this.card !== undefined) {
      if (!user.isAssigned) {
        const body: AddUserCard = {
          userId: user.id
        }
        this.userCardHttpService.addUserToCard(this.card.id, body).subscribe({
          next: (result: UserCard): void => {
            user.isAssigned = true;
          },
          error: (error: Error): void => {
            this.alertService.ErrorMessage(error.message);
          }
        })
      } else {
        this.userCardHttpService.removeUserFromCard(user.id, this.card.id).subscribe({
          next: (result: void): void => {
            user.isAssigned = false;
          },
          error: (error: Error): void => {
            this.alertService.ErrorMessage(error.message);
          }
        })
      }
    }
  }
}
