import { Component } from '@angular/core';
import { AlertService } from '../../core/services/alert.service';
import { UserHttpService } from '../../core/services/http/user-http.service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SessionService } from '../../core/services/session/session.service';
import { Router } from '@angular/router';
import {Location, NgForOf, NgIf} from '@angular/common';
import {InputComponent} from "../../shared/components/input/input.component";
import {BtnComponent} from '../../shared/components/btn/btn.component';
import {BtnIconComponent} from '../../shared/btn-icon/btn-icon.component';
import {TextareaComponent} from '../../shared/components/textarea/textarea.component';
import {BtnCloseComponent} from '../../shared/components/btn-close/btn-close.component';
import {AvatarComponent} from '../../shared/components/avatar/avatar.component';
import {ResourcesService} from '../../core/services/resources.service';
import {BtnThemeComponent} from '../../shared/components/btn-theme/btn-theme.component';
import {UpdateUser} from '../../core/models/user';

@Component({
  selector: 'app-user-menu',
  imports: [ReactiveFormsModule, InputComponent, BtnComponent, BtnIconComponent, TextareaComponent, BtnCloseComponent, AvatarComponent, NgIf, NgForOf, BtnThemeComponent],
  templateUrl: './user-menu.component.html',
  styleUrl: './user-menu.component.css'
})
export class UserMenuComponent {
  email: string | undefined ;
  username: string | undefined = undefined;
  avatarBackground: string | undefined = undefined;
  avatarBackgrounds: string[] = [];
  theme: string | undefined = undefined;

  constructor(private alertService: AlertService,
    private userHttpService: UserHttpService,
    private sessionServiceService: SessionService,
    private router: Router,
    private location: Location,
    private resourcesService: ResourcesService) { }

  ngOnInit(): void {
    this.email = this.sessionServiceService.getSessionData()?.email;
    this.username = this.sessionServiceService.getSessionData()?.username;
    this.avatarBackground = this.sessionServiceService.getSessionData()?.avatarBackground;
    this.avatarBackgrounds = this.resourcesService.avatarBackgrounds;
  }

  updatePasswordForm: FormGroup = new FormGroup({
    oldPassword: new FormControl('', [Validators.required, Validators.minLength(8)]),
    newPassword: new FormControl('', [Validators.required, Validators.minLength(8)])
  })

  onSubmitUpdatePassword(): void {
    if (this.updatePasswordForm.invalid) {
      if (this.updatePasswordForm.controls["oldPassword"].errors) {
        this.alertService.ErrorMessage('Old password must be at least 8 characters.');
      } else if (this.updatePasswordForm.controls["newPassword"].errors) {
        this.alertService.ErrorMessage('New password must be at least 8 characters.');
      }
      return;
    }
    const body: UpdateUser = {
      oldPassword: this.updatePasswordForm.value.oldPassword,
      newPassword: this.updatePasswordForm.value.newPassword
    }
    this.userHttpService.update(body).subscribe({
      next: (response: any): void => {
        this.alertService.SuccessMessage("Password changed successfully.");
      },
      error: (error: Error): void => {
        this.alertService.ErrorMessage(error.message);
      }
    });
  }

  updateUsernameForm: FormGroup = new FormGroup({
    username: new FormControl('', [Validators.required, Validators.pattern(/^\w+$/)])
  })

  onSubmitUpdateUsername(): void {
    if (this.updateUsernameForm.invalid) {
      if (this.updateUsernameForm.controls["username"].errors) {
        this.alertService.ErrorMessage('Invalid username. Only letters, numbers and underscore.');
      }
      return;
    }
    const body: UpdateUser = {
      username: this.updateUsernameForm.value.username
    }
    this.userHttpService.update(body).subscribe({
      next: (response: any): void => {
        this.sessionServiceService.updateSessionData({ username: response.username });
        this.alertService.SuccessMessage("Username changed successfully.");
      },
      error: (error: Error): void => {
        this.alertService.ErrorMessage(error.message);
      }
    });
  }

  onSubmitUpdateTheme(): void {
    const actualTheme: string | undefined = this.sessionServiceService.getSessionData()?.theme
    let theme: string = "Light";
    if (actualTheme == "Light" && actualTheme != undefined) {
      theme = "Dark";
    }
    else if (actualTheme == "Dark" && actualTheme != undefined) {
      theme = "Light";
    }
    const body: UpdateUser = {
      theme: theme
    }
    this.userHttpService.update(body).subscribe({
      next: (response: any): void => {
        this.sessionServiceService.updateSessionData({ theme: response.theme });
        const body: HTMLElement = document.querySelector('body') as HTMLElement;
        body.classList.remove('dark');
        body.classList.remove('light');
        body.classList.add(response.theme.toLowerCase());
        this.theme = theme;
      },
      error: (error: Error): void => {
        this.alertService.ErrorMessage(error.message);
      }
    });
  }

  onSubmitDeleteUser(): void {
    this.userHttpService.delete().subscribe({
      next: (response: any): void => {
        this.logout();
      },
      error: (error: Error): void => {
        this.alertService.ErrorMessage(error.message);
      }
    });
  }

  updateAvatar(avatarBackground: string) {
    const body: UpdateUser = {
      avatarBackground: avatarBackground
    }
    this.userHttpService.update(body).subscribe({
      next: (response: any): void => {
        this.sessionServiceService.updateSessionData({ avatarBackground: response.avatarBackground });
        this.avatarBackground = this.sessionServiceService.getSessionData()?.avatarBackground;
        this.alertService.SuccessMessage("Avatar changed successfully.");
      },
      error: (error: Error): void => {
        this.alertService.ErrorMessage(error.message);
      }
    });
  }

  logout(): void {
    this.sessionServiceService.clearSession();
    this.router.navigate(['/']);
  }

  goBack(): void {
    this.location.back();
  }
}
