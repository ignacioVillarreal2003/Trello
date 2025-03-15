import { Component } from '@angular/core';
import {Router} from '@angular/router';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {UserHttpService} from '../../core/services/http/user-http.service';
import {AlertService} from '../../core/services/alert.service';
import {NgIf} from '@angular/common';
import {SessionService} from '../../core/services/session/session.service';
import {InputComponent} from '../../shared/components/input/input.component';
import {BtnComponent} from '../../shared/components/btn/btn.component';
import {LoginUser, RegisterUser, UserAuth} from '../../core/models/user';
import {AddBoard} from '../../core/models/board';

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
              private sessionService: SessionService) { }

  formLogin: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email, Validators.maxLength(64)]),
    password: new FormControl('', [Validators.required, Validators.minLength(8), Validators.maxLength(64)])
  });

  onSubmitLogin(): void {
    if (!this.handleFormErrors(this.formLogin)) {
      const body: LoginUser = {
        email: this.formLogin.value.email,
        password: this.formLogin.value.password,
      }
      this.userHttpService.login(body).subscribe({
        next: (response: UserAuth): void => {
          this.saveUserData(response);
          this.applyTheme(response.user.theme)
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
      const body: RegisterUser = {
        email: this.formRegister.value.email,
        password: this.formRegister.value.password,
        username: this.formRegister.value.username,
      }
      this.userHttpService.register(body).subscribe({
        next: (response: UserAuth): void => {
          this.saveUserData(response);
          this.applyTheme(response.user.theme)
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
      theme: data.user.theme,
      avatarBackground: data.user.avatarBackground
    });
  }

  toggleAuthMode(): void {
    this.isLogin = !this.isLogin;
  }

  applyTheme(theme: string): void {
    const body: HTMLElement = document.querySelector('body') as HTMLElement;
    body.classList.remove('dark');
    body.classList.remove('light');
    if (theme) {
      body.classList.add(theme.toLowerCase());
    } else {
      body.classList.add('light');
    }
  }
}
