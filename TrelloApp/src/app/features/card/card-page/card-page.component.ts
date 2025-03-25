import { Component } from '@angular/core';
import {CardCommentsComponent} from '../card-comments/card-comments.component';
import {CardUsersComponent} from '../card-users/card-users.component';
import {CardLabelsComponent} from '../card-labels/card-labels.component';
import {CardDetailsComponent} from '../card-details/card-details.component';
import {BtnCloseComponent} from '../../../shared/components/btn-close/btn-close.component';
import {HeaderComponent} from '../../header/header.component';
import {Location, NgIf, NgSwitch, NgSwitchCase} from '@angular/common';
import {Card} from '../../../core/models/card';
import {ActivatedRoute, Router} from '@angular/router';
import {CardHttpService} from '../../../core/services/http/card-http.service';
import {CardUpdateTitleFormComponent} from '../card-update-title-form/card-update-title-form.component';
import {CardUpdateIsCompletedFormComponent} from '../card-update-is-completed-form/card-update-is-completed-form.component';
import {BtnDeleteCardComponent} from '../btn-delete-card/btn-delete-card.component';
import {BoardCookieService} from '../../../core/services/session/board-cookie.service';
import {BoardCommunicationService} from '../../../core/services/communication/board-communication.service';
import {CardCommunicationService} from '../../../core/services/communication/card-communication.service';
import {BoardHubService} from '../../../core/services/websocket/board-hub.service';

@Component({
  selector: 'app-card-page',
  imports: [
    CardCommentsComponent,
    CardUsersComponent,
    CardUsersComponent,
    CardLabelsComponent,
    CardDetailsComponent,
    BtnCloseComponent,
    HeaderComponent,
    NgIf,
    NgSwitchCase,
    NgSwitch,
    CardUpdateTitleFormComponent,
    CardUpdateIsCompletedFormComponent,
    BtnDeleteCardComponent
  ],
  templateUrl: './card-page.component.html',
  styleUrl: './card-page.component.css'
})
export class CardPageComponent {
  cardId: number | undefined = undefined;
  card: Card | undefined = undefined;
  currentMode: string = 'details';
  boardId: number | undefined = undefined;

  constructor(private location: Location,
              private boardCookieService: BoardCookieService,
              private route: ActivatedRoute,
              private cardHttpService: CardHttpService,
              private boardCommunicationService: BoardCommunicationService,
              private boardHubService: BoardHubService,
              private cardCommunicationService: CardCommunicationService,
              private router: Router) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params: any): void => {
      this.cardId = params.get('id');
      this.getCard();
    });
    this.boardId = this.boardCookieService.getCookie();
    if (this.boardId != undefined) {
      this.boardHubService.connectToBoard(this.boardId);
    }
    this.boardCommunicationService.deleteBoard$.subscribe((boardId: number | null): void => {
      if (boardId !== null && this.boardId != undefined && this.boardId == boardId) {
        this.router.navigate(['dashboard']);
      }
    });
    this.cardCommunicationService.deleteCard$.subscribe((cardId: number | null): void => {
      if (cardId != null) {
        this.goBack();
      }
    });
    this.cardCommunicationService.updateCard$.subscribe((card: Card | null): void => {
      if (card != null) {
        this.card = card;
      }
    })
  }

  getCard(): void {
    if (this.cardId !== undefined) {
      this.cardHttpService.getCardById(this.cardId).subscribe({
        next: (card: Card) => {
          this.card = card;
        }
      })
    }
  }

  setMode(mode: string): void {
    this.currentMode = mode;
  }

  goBack(): void {
    this.location.back();
  }
}
