import {Component, EventEmitter, Input, Output} from '@angular/core';
import {ReactiveFormsModule} from '@angular/forms';
import {Location, NgIf, NgSwitch, NgSwitchCase} from '@angular/common';
import {Card, UpdateCard} from '../../../core/models/card';
import {CardCommentsComponent} from '../card-comments/card-comments.component';
import {CardLabelsComponent} from '../card-labels/card-labels.component';
import {CardUsersComponent} from '../card-users/card-users.component';
import {CardDetailsComponent} from '../card-details/card-details.component';
import {HeaderComponent} from '../../header/header.component';
import {BtnCloseComponent} from '../../../shared/components/btn-close/btn-close.component';
import {CheckboxComponent} from '../../../shared/components/checkbox/checkbox.component';
import {ActivatedRoute} from '@angular/router';
import {CardHttpService} from '../../../core/services/http/card-http.service';
import {AlertService} from '../../../core/services/alert.service';
import {SessionService} from '../../../core/services/session/session.service';

@Component({
  selector: 'app-card-view',
  imports: [
    ReactiveFormsModule,
    NgSwitchCase,
    NgSwitch,
    CardCommentsComponent,
    CardLabelsComponent,
    CardUsersComponent,
    CardDetailsComponent,
    HeaderComponent,
    BtnCloseComponent,
    CheckboxComponent,
    NgIf
  ],
  templateUrl: './card-view.component.html',
  standalone: true,
  styleUrl: './card-view.component.css'
})
export class CardViewComponent {
  boardId: number | undefined = undefined;
  cardId: number | undefined = undefined;
  card: Card | undefined = undefined;
  currentMode: string = 'details';

  constructor(private location: Location,
              private sessionService: SessionService,
              private route: ActivatedRoute,
              private cardHttpService: CardHttpService,
              private alertService: AlertService) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params: any): void => {
      this.cardId = params.get('id');
      this.getCard();
    });
    this.boardId = this.sessionService.getBoardData()?.boardId;
  }

  getCard(): void {
    if (this.cardId !== undefined) {
      this.cardHttpService.getCardById(this.cardId).subscribe({
        next: (card: Card) => {
          this.card = card;
        },
        error: (error: Error) => {
          this.alertService.ErrorMessage(error.message)
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

  updateTitle($event: Event) {
    const input: HTMLInputElement = $event.target as HTMLInputElement;
    const title: string = input.value;
    if (this.cardId !== undefined) {
      const body: UpdateCard = {
        title: title
      }
      this.cardHttpService.update(this.cardId, body).subscribe({
        next: (card: Card) => {
          this.card = card;
        },
        error: (error: Error) => {
          this.alertService.ErrorMessage(error.message);
        }
      })
    }
  }

  deleteCard() {
    if (this.cardId !== undefined) {
      this.cardHttpService.delete(this.cardId).subscribe({
        next: (response: void) => {
          this.goBack();
        },
        error: (error: Error) => {
          this.alertService.ErrorMessage(error.message);
        }
      })
    }
  }

  updateIsCompleted() {
    if (this.card !== undefined) {
      const isCompleted: boolean = !this.card.isCompleted;
      const body: UpdateCard = {
        isCompleted: isCompleted
      }
      this.cardHttpService.update(this.card.id, body).subscribe({
        next: (response: Card) => {
          this.card = response;
        },
        error: (error: Error) => {
          this.alertService.ErrorMessage(error.message);
        }
      })
    }
  }
}
