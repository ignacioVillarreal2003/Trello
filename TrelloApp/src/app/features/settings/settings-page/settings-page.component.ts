import { Component } from '@angular/core';
import {BtnCloseComponent} from "../../../shared/components/btn-close/btn-close.component";
import {BtnComponent} from "../../../shared/components/btn/btn.component";
import {UserProfileComponent} from '../user-profile/user-profile.component';
import {ChangeUsernameFormComponent} from '../change-username-form/change-username-form.component';
import {ChangePasswordFormComponent} from '../change-password-form/change-password-form.component';
import {ChangeAvatarFormComponent} from '../change-avatar-form/change-avatar-form.component';
import {ChangeThemeFormComponent} from '../change-theme-form/change-theme-form.component';
import {SessionService} from '../../../core/services/session/session.service';
import {UserHttpService} from '../../../core/services/http/user-http.service';
import {Router} from '@angular/router';
import {Location} from '@angular/common';
import {HeaderComponent} from '../../header/header.component';

@Component({
  selector: 'app-settings-page',
  imports: [
    BtnCloseComponent,
    BtnComponent,
    UserProfileComponent,
    ChangeUsernameFormComponent,
    ChangePasswordFormComponent,
    ChangeAvatarFormComponent,
    ChangeThemeFormComponent,
    HeaderComponent
  ],
  templateUrl: './settings-page.component.html',
  styleUrl: './settings-page.component.css'
})
export class SettingsPageComponent {

  constructor(private sessionService: SessionService,
              private userHttpService: UserHttpService,
              private location: Location,
              private router: Router) { }

  onSubmitDeleteUser(): void {
    this.userHttpService.delete().subscribe({
      next: (response: any): void => {
        this.logout();
      }
    });
  }

  logout(): void {
    this.sessionService.clearSession();
    this.router.navigate(['/']);
  }

  goBack(): void {
    this.location.back();
  }
}
