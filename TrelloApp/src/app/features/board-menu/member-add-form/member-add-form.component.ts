import {Component, Input} from '@angular/core';
import {BtnComponent} from "../../../shared/components/btn/btn.component";
import {InputComponent} from "../../../shared/components/input/input.component";
import {NgForOf} from "@angular/common";
import {User} from '../../../core/models/user';
import {AddUserBoard, UserBoard} from '../../../core/models/user-board';
import {Board} from '../../../core/models/board';
import {UserHttpService} from '../../../core/services/http/user-http.service';
import {UserBoardHttpService} from '../../../core/services/http/user-board-http.service';
import {UserBoardCommunicationService} from '../../../core/services/communication/user-board-communication.service';

@Component({
  selector: 'app-member-add-form',
    imports: [
        BtnComponent,
        InputComponent,
        NgForOf
    ],
  templateUrl: './member-add-form.component.html',
  styleUrl: './member-add-form.component.css'
})
export class MemberAddFormComponent {
  @Input() board: Board | undefined = undefined;
  usersByUsername: User[] = [];

  constructor(private userHttpService: UserHttpService,
              private userBoardHttpService: UserBoardHttpService) {}

  getUsersByUsername($event: Event): void {
    const inputElement: HTMLInputElement = $event.target as HTMLInputElement;
    const username: string = inputElement.value;
    if (username.length > 0) {
      this.userHttpService.getUsersByUsername(username).subscribe({
        next: (result: User[]): void => {
          this.usersByUsername = result;
        }
      })
    } else {
      this.usersByUsername = [];
    }
  }

  addUserToBoard(user: User): void {
    if (this.board !== undefined) {
      const body: AddUserBoard = {
        userId: user.id,
        role: "Member"
      }

      this.userBoardHttpService.addUserToBoard(this.board.id, body).subscribe();
    }
  }
}
