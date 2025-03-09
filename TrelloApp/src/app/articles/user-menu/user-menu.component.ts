import { Component } from '@angular/core';
import { AlertService } from '../../core/services/alert.service';
import { UserHttpService } from '../../core/services/http/user-http.service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { SessionServiceService } from '../../core/services/session/session-service.service';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import {InputComponent} from "../../shared/components/input/input.component";
import {BtnComponent} from '../../shared/components/btn/btn.component';
import {BtnIconComponent} from '../../shared/btn-icon/btn-icon.component';
import {TextareaComponent} from '../../shared/components/textarea/textarea.component';
import {BtnCloseComponent} from '../../shared/components/btn-close/btn-close.component';

@Component({
  selector: 'app-user-menu',
  imports: [ReactiveFormsModule, InputComponent, BtnComponent, BtnIconComponent, TextareaComponent, BtnCloseComponent],
  templateUrl: './user-menu.component.html',
  styleUrl: './user-menu.component.css'
})
export class UserMenuComponent {

  constructor(private alertService: AlertService,
    private userHttpService: UserHttpService,
    private sessionServiceService: SessionServiceService,
    private router: Router,
    private location: Location) { }

  ngOnInit(): void {
    this.email = this.sessionServiceService.getSessionData()?.email ?? "";
    this.username = this.sessionServiceService.getSessionData()?.username ?? "";
  }

  /* User data*/
  email: string = "";
  username: string = "";

  /* Update password */
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
    this.userHttpService.updatePassword(this.updatePasswordForm.value.oldPassword, this.updatePasswordForm.value.newPassword).subscribe({
      next: (response: any): void => {
        this.alertService.SuccessMessage("Password changed successfully.");
      },
      error: (error: HttpErrorResponse): void => {
        const errorMessage: string = error?.message || 'Error in the server. Try again later.';
        this.alertService.ErrorMessage(errorMessage);
      }
    });
  }

  /* Update username */
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
    this.userHttpService.updateUsername(this.updateUsernameForm.value.username).subscribe({
      next: (response: any): void => {
        this.sessionServiceService.updateSessionData({ username: response.username });
        this.alertService.SuccessMessage("Username changed successfully.");
      },
      error: (error: HttpErrorResponse): void => {
        const errorMessage: string = error?.message || 'Error in the server. Try again later.';
        this.alertService.ErrorMessage(errorMessage);
      }
    });
  }

  /* Update theme */
  onSubmitUpdateTheme(): void {
    const actualTheme: string | undefined = this.sessionServiceService.getSessionData()?.theme
    let theme: string = "Light";
    if (actualTheme == "Light" && actualTheme != undefined) {
      theme = "Dark";
    }
    else if (actualTheme == "Dark" && actualTheme != undefined) {
      theme = "Light";
    }
    this.userHttpService.updateTheme(theme).subscribe({
      next: (response: any): void => {
        this.sessionServiceService.updateSessionData({ theme: response.theme });
        const body: HTMLElement = document.querySelector('body') as HTMLElement;
        body.classList.remove('dark');
        body.classList.remove('light');
        body.classList.add(response.theme.toLowerCase());
      },
      error: (error: HttpErrorResponse): void => {
        const errorMessage: string = error?.message || 'Error in the server. Try again later.';
        this.alertService.ErrorMessage(errorMessage);
      }
    });
  }

  /* Delete user */
  onSubmitDeleteUser(): void {
    this.userHttpService.delete().subscribe({
      next: (response: any): void => {
        this.logout();
      },
      error: (error: HttpErrorResponse): void => {
        const errorMessage: string = error?.message || 'Error in the server. Try again later.';
        this.alertService.ErrorMessage(errorMessage);
      }
    });
  }

  /* Logout */
  logout(): void {
    this.sessionServiceService.clearSession();
    this.router.navigate(['/']);
  }

  /* Close */
  goBack(): void {
    this.location.back();
  }
}
