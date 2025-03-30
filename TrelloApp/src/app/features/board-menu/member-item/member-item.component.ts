import {Component, Input} from '@angular/core';
import {BtnComponent} from "../../../shared/components/btn/btn.component";
import {User} from '../../../core/models/user';
import {Board} from '../../../core/models/board';
import {UserBoardHttpService} from '../../../core/services/http/user-board-http.service';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-member-item',
  imports: [
    BtnComponent,
    NgIf
  ],
  templateUrl: './member-item.component.html',
  styleUrl: './member-item.component.css'
})
export class MemberItemComponent {
  @Input() user: User | undefined = undefined;
  @Input() board: Board | undefined = undefined;

  constructor(private userBoardHttpService: UserBoardHttpService) {}

  removeUserFromBoard(): void {
    if (this.board !== undefined && this.user !== undefined) {
      this.userBoardHttpService.removeUserFromBoard(this.board.id, this.user.id).subscribe();
    }
  }
}
