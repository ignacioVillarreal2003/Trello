import {Component, Input} from '@angular/core';
import {AvatarComponent} from "../../../shared/components/avatar/avatar.component";
import {NgForOf} from "@angular/common";
import {UserHttpService} from '../../../core/services/http/user-http.service';
import {SessionService} from '../../../core/services/session/session.service';
import {ResourcesService} from '../../../core/services/resources.service';
import {UpdateUser, User} from '../../../core/models/user';
import {UserCommunicationService} from '../../../core/services/communication/user-communication.service';

@Component({
  selector: 'app-change-avatar-form',
    imports: [
        AvatarComponent,
        NgForOf
    ],
  templateUrl: './change-avatar-form.component.html',
  styleUrl: './change-avatar-form.component.css'
})
export class ChangeAvatarFormComponent {
  avatarBackgrounds: string[] = [];
  @Input() username: string | undefined;

  constructor(private sessionService: SessionService,
              private resourcesService: ResourcesService,
              private userHttpService: UserHttpService,
              private userCommunicationService: UserCommunicationService) { }

  ngOnInit(): void {
    this.username = this.sessionService.getSessionData()?.username;
    this.avatarBackgrounds = this.resourcesService.avatarBackgrounds;
  }

  onSubmitUpdateAvatar(avatarBackground: string) {
    const body: UpdateUser = {
      avatarBackground: avatarBackground
    }
    this.userHttpService.update(body).subscribe({
      next: (response: User): void => {
        this.sessionService.updateSessionData({ avatarBackground: response.avatarBackground });
        this.userCommunicationService.setUpdateUser(response);
      }
    });
  }
}
