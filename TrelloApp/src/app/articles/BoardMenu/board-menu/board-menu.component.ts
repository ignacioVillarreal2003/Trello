import {Component, EventEmitter, Input, Output} from '@angular/core';
import {NgForOf, NgIf, NgStyle, NgSwitch, NgSwitchCase} from '@angular/common';
import {AlertService} from '../../../core/services/alert.service';
import {BoardHttpService} from '../../../core/services/http/board-http.service';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {ResourcesService} from '../../../core/services/resources.service';
import {CommunicationService} from '../../../core/services/communication.service';
import {BtnComponent} from '../../../shared/components/btn/btn.component';
import {BtnCloseComponent} from '../../../shared/components/btn-close/btn-close.component';
import {BtnBackComponent} from '../../../shared/components/btn-back/btn-back.component';
import {BoardBackgroundComponent} from '../board-background/board-background.component';
import {BoardLabelComponent} from '../board-label/board-label.component';
import {Board, UpdateBoard} from '../../../core/models/board';
import {BoardMemberComponent} from '../board-member/board-member.component';
import {LoginUser} from '../../../core/models/user';

@Component({
  selector: 'app-board-menu',
  imports: [
    NgIf,
    BtnComponent,
    BtnCloseComponent,
    BtnBackComponent,
    NgSwitchCase,
    NgSwitch,
    BoardBackgroundComponent,
    BoardLabelComponent,
    BoardMemberComponent,
  ],
  templateUrl: './board-menu.component.html',
  standalone: true,
  styleUrl: './board-menu.component.css'
})
export class BoardMenuComponent {
  @Input() board: Board | undefined = undefined;
  @Input() isOpen = false;
  @Output() public close = new EventEmitter<void>();
  @Input() boardId: number | undefined = undefined;
  currentMode: string = 'menu';

  constructor() {}

  onClose() {
    this.isOpen = false;
    this.close.emit();
  }

  setMode(mode: string): void {
    this.currentMode = mode;
  }
}
