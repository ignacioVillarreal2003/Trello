import {Component, Input} from '@angular/core';
import {NgForOf} from "@angular/common";
import {Board} from '../../../core/models/board';
import {User} from '../../../core/models/user';
import {UserBoardHttpService} from '../../../core/services/http/user-board-http.service';
import {MemberItemComponent} from '../member-item/member-item.component';
import {UserBoardCommunicationService} from '../../../core/services/communication/user-board-communication.service';
import {UserBoard} from '../../../core/models/user-board';

@Component({
  selector: 'app-member-list',
  imports: [
    NgForOf,
    MemberItemComponent
  ],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css'
})
export class MemberListComponent {
  @Input() board: Board | undefined = undefined;
  usersFromBoard: User[] = [];

  constructor(private userBoardHttpService: UserBoardHttpService,
              private userBoardCommunicationService: UserBoardCommunicationService) {
  }

  ngOnInit(): void {
    this.getUsersByBoardId();
    this.userBoardCommunicationService.addUserBoard$.subscribe((userBoard: UserBoard | null): void => {
      if (userBoard !== null) {
        this.usersFromBoard.push(userBoard.user);
      }
    });
    this.userBoardCommunicationService.deleteUserBoard$.subscribe((value: { userId: number, boardId: number } | null): void => {
      if (value !== null) {
        this.usersFromBoard = this.usersFromBoard.filter(u => u.id !== value.userId);
      }
    });
  }

  getUsersByBoardId(): void {
    if (this.board !== undefined) {
      this.userBoardHttpService.getUsersByBoardId(this.board.id).subscribe({
        next: (result: User[]): void => {
          this.usersFromBoard = result;
        }
      })
    }
  }
}
