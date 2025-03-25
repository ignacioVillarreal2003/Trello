import {Component, EventEmitter, Input, Output} from '@angular/core';
import {BtnBackComponent} from "../../../shared/components/btn-back/btn-back.component";
import {BtnCloseComponent} from "../../../shared/components/btn-close/btn-close.component";
import {BtnComponent} from "../../../shared/components/btn/btn.component";
import {NgIf, NgSwitch, NgSwitchCase} from "@angular/common";
import {Board} from '../../../core/models/board';
import {MenuBackgroundComponent} from '../menu-background/menu-background.component';
import {MenuLabelComponent} from '../menu-label/menu-label.component';
import {MenuMemberComponent} from '../menu-member/menu-member.component';
import {BtnDeleteBoardComponent} from '../btn-delete-board/btn-delete-board.component';
import {BoardCommunicationService} from '../../../core/services/communication/board-communication.service';

@Component({
  selector: 'app-board-menu-page',
  imports: [
    BtnBackComponent,
    BtnCloseComponent,
    BtnComponent,
    NgIf,
    NgSwitchCase,
    MenuBackgroundComponent,
    MenuLabelComponent,
    MenuMemberComponent,
    NgSwitch,
    BtnDeleteBoardComponent
  ],
  templateUrl: './board-menu-page.component.html',
  styleUrl: './board-menu-page.component.css'
})
export class BoardMenuPageComponent {
  @Input() board: Board | undefined = undefined;
  @Input() isOpen = false;
  @Output() public close = new EventEmitter<void>();
  currentMode: string = 'menu';

  onClose() {
    this.isOpen = false;
    this.close.emit();
  }

  setMode(mode: string): void {
    this.currentMode = mode;
  }
}
