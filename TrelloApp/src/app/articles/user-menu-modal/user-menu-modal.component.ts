import {Component} from '@angular/core';
import {AlertService} from '../../core/services/alert.service';
import {UserHttpService} from '../../core/services/http/user-http.service';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {HttpErrorResponse, HttpResponse} from '@angular/common/http';
import {ThemeBtnComponent} from '../../shared/buttons/theme-btn/theme-btn.component';
import {SessionServiceService} from '../../core/services/session-service.service';
import {Router} from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-user-menu-modal',
  imports: [
    ReactiveFormsModule,
    ThemeBtnComponent
  ],
  templateUrl: './user-menu-modal.component.html',
  standalone: true,
  styleUrl: './user-menu-modal.component.css'
})
export class UserMenuModalComponent {

  constructor(private alertService: AlertService,
              private userHttpService: UserHttpService,
              private sessionServiceService: SessionServiceService,
              private router: Router,
              private location: Location) {}

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
        this.sessionServiceService.setUserData({
          email: response.user.email,
          username: response.user.username,
          theme: response.user.theme
        });
        this.alertService.SuccessMessage("Username changed successfully.");
      },
      error: (error: HttpErrorResponse): void => {
        const errorMessage: string = error?.message || 'Error in the server. Try again later.';
        this.alertService.ErrorMessage(errorMessage);
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
