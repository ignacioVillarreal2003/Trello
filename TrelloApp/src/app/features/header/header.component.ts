import {Component, Input} from '@angular/core';
import {RouterLink} from '@angular/router';
import {SessionService} from '../../core/services/session/session.service';
import {Subscription} from 'rxjs';
import {NgIf} from '@angular/common';
import {AvatarComponent} from '../../shared/components/avatar/avatar.component';
import {User} from '../../core/models/user';
import {UserCommunicationService} from '../../core/services/communication/user-communication.service';

@Component({
  selector: 'app-header',
  imports: [
    RouterLink,
    NgIf,
    AvatarComponent,
  ],
  templateUrl: './header.component.html',
  standalone: true,
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  username: string | undefined = undefined;
  avatarBackground: string | undefined = undefined;

  constructor(private sessionServiceService: SessionService,
              private userCommunicationService: UserCommunicationService) {}

  ngOnInit(): void {
    this.username = this.sessionServiceService.getSessionData()?.username;
    this.avatarBackground = this.sessionServiceService.getSessionData()?.avatarBackground;
    this.userCommunicationService.updateUser$.subscribe((user: User | null): void => {
      if (user !== null) {
        this.username = user.username;
        this.avatarBackground = user.avatarBackground;
      }
    })
  }
}
