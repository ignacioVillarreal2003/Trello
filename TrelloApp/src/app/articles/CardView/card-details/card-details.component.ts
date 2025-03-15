import {Component, Input} from '@angular/core';
import {NgForOf, NgIf} from '@angular/common';
import {Card, UpdateCard} from '../../../core/models/card';
import {Label} from '../../../core/models/label';
import {User} from '../../../core/models/user';
import {AvatarComponent} from '../../../shared/components/avatar/avatar.component';
import {AlertService} from '../../../core/services/alert.service';
import {CardHttpService} from '../../../core/services/http/card-http.service';
import {CardLabelHttpService} from '../../../core/services/http/card-label-http.service';
import {UserCardHttpService} from '../../../core/services/http/user-card-http.service';

@Component({
  selector: 'app-card-details',
  imports: [
    NgIf,
    NgForOf,
    AvatarComponent
  ],
  templateUrl: './card-details.component.html',
  styleUrl: './card-details.component.css'
})
export class CardDetailsComponent {
  @Input() card: Card | undefined = undefined;
  @Input() boardId: number | undefined = undefined;

  isDescriptionActive: boolean = false;
  users: User[] = [];
  labels: Label[] = [];

  constructor(private alertService: AlertService,
              private cardLabelHttpService: CardLabelHttpService,
              private userCardHttpService: UserCardHttpService,
              private cardHttpService: CardHttpService) {
  }

  ngOnInit(): void {
    if (this.card !== undefined) {
      this.isDescriptionActive = this.card.description.length > 0;
    }
    this.getLabels();
    this.getUsers();
  }

  updateCardDescription($event: Event) {
    if (this.card !== undefined) {
      const input: HTMLInputElement = $event.target as HTMLInputElement;
      const value = input.value;
      const body: UpdateCard = {
        description: value
      }
      this.cardHttpService.update(this.card.id, body).subscribe({
        next: (result: Card) => {
          this.card = result;
        },
        error: (error: Error) => {
          this.alertService.ErrorMessage(error.message)
        }
      })
    }
  }

  getLabels(): void {
    if (this.card !== undefined) {
      this.cardLabelHttpService.getLabelsByCardId(this.card.id).subscribe({
        next: (result: Label[]): void => {
          this.labels = result;
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message);
        }
      });
    }
  }

  getUsers(): void {
    if (this.card !== undefined) {
      this.userCardHttpService.getUsersByCardId(this.card.id).subscribe({
        next: (result: User[]): void => {
          this.users = result;
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message);
        }
      });
    }
  }
}
