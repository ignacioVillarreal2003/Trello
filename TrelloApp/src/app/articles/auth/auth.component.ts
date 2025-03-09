import { Component } from '@angular/core';
import {Router} from '@angular/router';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {UserHttpService} from '../../core/services/http/user-http.service';
import {AlertService} from '../../core/services/alert.service';
import {NgIf} from '@angular/common';
import {HttpErrorResponse} from '@angular/common/http';
import {SessionServiceService} from '../../core/services/session/session-service.service';
import {InputComponent} from '../../shared/components/input/input.component';
import {BtnComponent} from '../../shared/components/btn/btn.component';
import {UserAuth} from '../../core/models/user';

@Component({
  selector: 'app-auth',
  imports: [
    ReactiveFormsModule,
    NgIf,
    InputComponent,
    BtnComponent
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
              private sessionService: SessionServiceService) { }

  formLogin: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email, Validators.maxLength(64)]),
    password: new FormControl('', [Validators.required, Validators.minLength(8), Validators.maxLength(64)])
  });

  onSubmitLogin(): void {
    if (!this.handleFormErrors(this.formLogin)) {
      this.userHttpService.login(this.formLogin.value.email, this.formLogin.value.password).subscribe({
        next: (response: UserAuth): void => {
          this.saveUserData(response);
          this.router.navigate(['/board-dashboard']);
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message);
        }
      });
    }
  }

  formRegister: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email, Validators.maxLength(64)]),
    password: new FormControl('', [Validators.required, Validators.minLength(8), Validators.maxLength(64)]),
    username: new FormControl('', [Validators.required, Validators.maxLength(64)])
  });

  onSubmitRegister(): void {
    if (!this.handleFormErrors(this.formRegister)) {
      this.userHttpService.register(this.formRegister.value.email, this.formRegister.value.username, this.formRegister.value.password).subscribe({
        next: (response: UserAuth): void => {
          this.saveUserData(response);
          this.router.navigate(['/board-dashboard']);
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message);
        }
      });
    }
  }

  private handleFormErrors(form: FormGroup): boolean {
    if (form.invalid) {
      if (form.controls['email'].errors) {
        this.alertService.ErrorMessage('Invalid email address.');
      } else if (form.controls['password'].errors) {
        this.alertService.ErrorMessage('Password must have since 8 to 64 characters.');
      } else if (form.controls['username'] && form.controls['username'].errors) {
        this.alertService.ErrorMessage('Username must have less than 64 characters.');
      }
      return true;
    }
    return false;
  }

  saveUserData(data: UserAuth): void {
    this.sessionService.setSessionData({
      accessToken: data.accessToken,
      refreshToken: data.refreshToken,
      email: data.user.email,
      username: data.user.username,
      theme: data.user.theme
    });
  }

  toggleAuthMode(): void {
    this.isLogin = !this.isLogin;
  }
}
