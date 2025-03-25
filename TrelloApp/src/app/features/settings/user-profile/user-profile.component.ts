import { Component } from '@angular/core';
import {AvatarComponent} from "../../../shared/components/avatar/avatar.component";
import {NgIf} from "@angular/common";
import {SessionService} from '../../../core/services/session/session.service';
import {UserCommunicationService} from '../../../core/services/communication/user-communication.service';
import {User} from '../../../core/models/user';

@Component({
  selector: 'app-user-profile',
    imports: [
        AvatarComponent,
        NgIf
    ],
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.css'
})
export class UserProfileComponent {
  email: string | undefined ;
  username: string | undefined = undefined;
  avatarBackground: string | undefined = undefined;

  constructor(private sessionServiceService: SessionService,
              private userCommunicationService: UserCommunicationService) { }

  ngOnInit(): void {
    this.email = this.sessionServiceService.getSessionData()?.email;
    this.username = this.sessionServiceService.getSessionData()?.username;
    this.avatarBackground = this.sessionServiceService.getSessionData()?.avatarBackground;
    this.userCommunicationService.updateUser$.subscribe((user: User | null): void => {
      if (user !== null) {
        this.username = user.username;
        this.email = user.email;
        this.avatarBackground = user.avatarBackground;
      }
    });
  }
}
