import { Component } from '@angular/core';
import {BtnComponent} from "../../../shared/components/btn/btn.component";
import {InputComponent} from "../../../shared/components/input/input.component";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {Router} from '@angular/router';
import {UserHttpService} from '../../../core/services/http/user-http.service';
import {SessionService} from '../../../core/services/session/session.service';
import {ThemeService} from '../../../core/services/theme.service';
import {RegisterUser, UserAuth} from '../../../core/models/user';

@Component({
  selector: 'app-register-form',
    imports: [
        BtnComponent,
        InputComponent,
        ReactiveFormsModule
    ],
  templateUrl: './register-form.component.html',
  styleUrl: './register-form.component.css'
})
export class RegisterFormComponent {

  formRegister: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email, Validators.maxLength(64)]),
    password: new FormControl('', [Validators.required, Validators.minLength(8), Validators.maxLength(64)]),
    username: new FormControl('', [Validators.required, Validators.maxLength(64)])
  });

  errorMessages: any = {
    email: {
      required: 'Email is required.',
      email: 'Enter a valid email.',
      maxlength: 'The email must be less than 64 characters.'
    },
    password: {
      required: 'Password is required.',
      minlength: 'The password must be at least 8 characters long.',
      maxlength: 'The password must be less than 64 characters.'
    },
    username: {
      required: 'Username is required.',
      maxlength: 'The username must be less than 64 characters.'
    }
  };

  constructor(private router: Router,
              private userHttpService: UserHttpService,
              private sessionService: SessionService,
              private themeService: ThemeService) {}

  getErrorMessage(controlName: string): string {
    const control = this.formRegister.get(controlName);
    if (control?.errors) {
      for (const error in control.errors) {
        if (this.errorMessages[controlName][error]) {
          return this.errorMessages[controlName][error];
        }
      }
    }
    return '';
  }

  onSubmitRegister(): void {
    if (this.formRegister.invalid) {
      this.formRegister.markAllAsTouched();
      return;
    }

    const body: RegisterUser = {
      email: this.formRegister.value.email,
      password: this.formRegister.value.password,
      username: this.formRegister.value.username,
    };

    this.userHttpService.register(body).subscribe({
      next: (result: UserAuth) => {
        this.sessionService.setSessionData({
          accessToken: result.accessToken,
          refreshToken: result.refreshToken,
          email: result.user.email,
          username: result.user.username,
          theme: result.user.theme,
          avatarBackground: result.user.avatarBackground
        });
        this.themeService.applyTheme(result.user.theme);
        this.router.navigate(['/dashboard']);
      }
    });
  }
}
