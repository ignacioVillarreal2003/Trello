import {Component, Input} from '@angular/core';
import {BtnComponent} from '../../../shared/components/btn/btn.component';
import {NgForOf} from '@angular/common';
import {InputComponent} from '../../../shared/components/input/input.component';
import {Board} from '../../../core/models/board';
import {User} from '../../../core/models/user';
import {UserHttpService} from '../../../core/services/http/user-http.service';
import {UserBoardHttpService} from '../../../core/services/http/user-board-http.service';
import {AlertService} from '../../../core/services/alert.service';
import {AddUserBoard} from "../../../core/models/user-board";

@Component({
  selector: 'app-board-member',
  imports: [
    BtnComponent,
    NgForOf,
    InputComponent
  ],
  templateUrl: './board-member.component.html',
  styleUrl: './board-member.component.css'
})
export class BoardMemberComponent {
  @Input() board: Board | undefined = undefined;
  usersByUsername: User[] = [];
  usersFromBoard: User[] = [];

  constructor(private userHttpService: UserHttpService,
              private userBoardHttpService: UserBoardHttpService,
              private alertService: AlertService) {
  }

  ngOnInit(): void {
    this.getUsersByBoardId()
  }

  getUsersByUsername($event: Event): void {
    const inputElement: HTMLInputElement = $event.target as HTMLInputElement;
    const username: string = inputElement.value;
    if (username.length > 0) {
      this.userHttpService.getUsersByUsername(username).subscribe({
        next: (result: User[]): void => {
          this.usersByUsername = result;
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message)
        }
      })
    } else {
      this.usersByUsername = [];
    }
  }

  addUserToBoard(userId: number): void {
    if (this.board !== undefined) {
      const body: AddUserBoard = {
        userId: userId,
        role: "Member"
      }
      this.userBoardHttpService.add(this.board.id, body).subscribe({
        next: () => {
          this.alertService.SuccessMessage("User added successfully.");
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message)
        }
      })
    }
  }

  getUsersByBoardId(): void {
    if (this.board !== undefined) {
      this.userBoardHttpService.getUsersByBoardId(this.board.id).subscribe({
        next: (result: User[]): void => {
          this.usersFromBoard = result;
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message)
        }
      })
    }
  }

  removeUserFromBoard(userId: number): void {
    if (this.board !== undefined) {
      this.userBoardHttpService.delete(this.board.id, userId).subscribe({
        next: () => {
          this.usersFromBoard = this.usersFromBoard.filter(user => user.id !== userId);
          this.alertService.SuccessMessage("User removed successfully.");
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message)
        }
      })
    }
  }
}
