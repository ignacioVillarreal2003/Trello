import { Component } from '@angular/core';
import {Router} from '@angular/router';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {UserHttpService} from '../../core/services/http/user-http.service';
import {AlertService} from '../../core/services/alert.service';
import {NgIf} from '@angular/common';
import {HttpErrorResponse} from '@angular/common/http';
import {SessionServiceService} from '../../core/services/session-service.service';

@Component({
  selector: 'app-auth',
  imports: [
    ReactiveFormsModule,
    NgIf
  ],
  templateUrl: './auth.component.html',
  standalone: true,
  styleUrl: './auth.component.css'
})
export class AuthComponent {
  isLogin: boolean = true;

  constructor(private router: Router,
              private userHttpService: UserHttpService,
              private alertService: AlertService,
              private sessionServiceService: SessionServiceService) { }

  formLogin: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)]),
  });

  onSubmitLogin(): void {
    if (this.formLogin.invalid) {
      if (this.formLogin.controls["email"].errors) {
        this.alertService.ErrorMessage('Invalid email address.');
      } else if (this.formLogin.controls["password"].errors) {
        this.alertService.ErrorMessage('Password must be at least 8 characters.');
      }
      return;
    }
    this.userHttpService.login(this.formLogin.value.email, this.formLogin.value.password).subscribe({
      next: (response: any): void => {
        this.sessionServiceService.setToken(response.token);
        this.sessionServiceService.setUserData({
          email: response.email,
          username: response.username,
          theme: response.theme
        });
        this.router.navigate(['/board-dashboard']);
      },
      error: (error: HttpErrorResponse): void => {
        const errorMessage: string = error?.message || 'Error in the server. Try again later.';
        this.alertService.ErrorMessage(errorMessage);
      }
    });
  }

  formRegister: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(8)]),
    username: new FormControl('', [Validators.required, Validators.pattern(/^\w+$/)])
  });

  onSubmitRegister(): void {
    if (this.formRegister.invalid) {
      if (this.formRegister.controls["email"].errors) {
        this.alertService.ErrorMessage('Invalid email address.');
      } else if (this.formRegister.controls["password"].errors) {
        this.alertService.ErrorMessage('Password must be at least 8 characters.');
      } else if (this.formRegister.controls["username"].errors) {
        this.alertService.ErrorMessage('Invalid username. Only letters, numbers and underscore.');
      }
      return;
    }
    this.userHttpService.register(this.formRegister.value.email, this.formRegister.value.username, this.formRegister.value.password).subscribe({
      next: (response: any): void => {
        this.sessionServiceService.setToken(response.token);
        this.sessionServiceService.setUserData({
          email: response.email,
          username: response.username,
          theme: response.theme
        });
        this.router.navigate(['/board-dashboard']);
      },
      error: (error: HttpErrorResponse): void => {
        const errorMessage: string = error?.message || 'Error in the server. Try again later.';
        this.alertService.ErrorMessage(errorMessage);
      }
    });
  }
}
