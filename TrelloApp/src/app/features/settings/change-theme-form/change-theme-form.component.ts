import { Component } from '@angular/core';
import {BtnThemeComponent} from "../../../shared/components/btn-theme/btn-theme.component";
import {UserHttpService} from '../../../core/services/http/user-http.service';
import {SessionService} from '../../../core/services/session/session.service';
import {UpdateUser, User} from '../../../core/models/user';
import {UserCommunicationService} from '../../../core/services/communication/user-communication.service';

@Component({
  selector: 'app-change-theme-form',
    imports: [
        BtnThemeComponent
    ],
  templateUrl: './change-theme-form.component.html',
  styleUrl: './change-theme-form.component.css'
})
export class ChangeThemeFormComponent {
  theme: string | undefined = undefined;

  constructor(private userHttpService: UserHttpService,
              private sessionService: SessionService,
              private userCommunicationService: UserCommunicationService) { }

  ngOnInit(): void {
    this.theme = this.sessionService.getSessionData()?.theme;
  }

  onSubmitUpdateTheme(): void {
    let theme: string = "Light";
    if (this.theme == "Light" &&  this.theme != undefined) {
      theme = "Dark";
    }
    else if (this.theme == "Dark" && this.theme != undefined) {
      theme = "Light";
    }

    const body: UpdateUser = {
      theme: theme
    }

    this.userHttpService.update(body).subscribe({
      next: (result: User): void => {
        this.sessionService.updateSessionData({ theme: result.theme });
        this.userCommunicationService.setUpdateUser(result);
        this.theme = theme;
      }
    });
  }
}
