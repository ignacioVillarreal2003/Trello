import { Component } from '@angular/core';
import {BtnMenuComponent} from "../../../shared/components/btn-menu/btn-menu.component";
import {HeaderComponent} from "../../header/header.component";
import {NgIf} from "@angular/common";
import {Board} from '../../../core/models/board';
import {ActivatedRoute, Router} from '@angular/router';
import {BoardHttpService} from '../../../core/services/http/board-http.service';
import {ResourcesService} from '../../../core/services/resources.service';
import {ListListComponent} from '../list-list/list-list.component';
import {BoardMenuPageComponent} from '../../board-menu/board-menu-page/board-menu-page.component';
import {ReactiveFormsModule} from '@angular/forms';
import {BoardCommunicationService} from '../../../core/services/communication/board-communication.service';
import {BoardUpdateTitleFormComponent} from '../board-update-title-form/board-update-title-form.component';
import {BoardHubService} from '../../../core/services/websocket/board-hub.service';
import {BoardCookieService} from '../../../core/services/session/board-cookie.service';

@Component({
  selector: 'app-board-page',
  imports: [
    BtnMenuComponent,
    HeaderComponent,
    NgIf,
    ListListComponent,
    BoardMenuPageComponent,
    ReactiveFormsModule,
    BoardUpdateTitleFormComponent
  ],
  templateUrl: './board-page.component.html',
  styleUrl: './board-page.component.css'
})
export class BoardPageComponent {
  boardId: number | undefined = undefined;
  board: Board | undefined = undefined;
  boardBackgroundPath: string = "";
  isOpenBoardMenu: boolean = false;

  constructor(private route: ActivatedRoute,
              private boardHttpService: BoardHttpService,
              private resourcesService: ResourcesService,
              private boardCommunicationService: BoardCommunicationService,
              private boardHubService: BoardHubService,
              private boardCookieService: BoardCookieService,
              private router: Router) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params: any): void => {
      this.boardId = params.get('id');
      if (this.boardId !== undefined) {
        this.getBoard();
        this.boardHubService.connectToBoard(this.boardId);
        this.boardCookieService.setCookie(this.boardId)
      }
    });
    this.boardBackgroundPath = this.resourcesService.boardBackgroundPath;
    this.boardCommunicationService.updateBoard$.subscribe((board: Board | null): void => {
      if (board != null && this.board != undefined && this.board.id == board.id) {
        this.board = board;
      }
    });
    this.boardCommunicationService.deleteBoard$.subscribe((boardId: number | null): void => {
      if (boardId !== null && this.board != undefined && this.board.id == boardId) {
        this.router.navigate(['dashboard']);
      }
    });
  }

  getBoard(): void {
    if (this.boardId) {
      this.boardHttpService.getBoard(this.boardId).subscribe({
        next: (result: Board): void => {
          this.board = result;
        }
      });
    }
  }
}
