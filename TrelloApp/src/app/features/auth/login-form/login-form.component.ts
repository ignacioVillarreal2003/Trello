import { Component } from '@angular/core';
import {BtnComponent} from "../../../shared/components/btn/btn.component";
import {InputComponent} from "../../../shared/components/input/input.component";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {LoginUser, UserAuth} from '../../../core/models/user';
import {ActivatedRoute, Router} from '@angular/router';
import {UserHttpService} from '../../../core/services/http/user-http.service';
import {SessionService} from '../../../core/services/session/session.service';
import {ThemeService} from '../../../core/services/theme.service';

@Component({
  selector: 'app-login-form',
    imports: [
        BtnComponent,
        InputComponent,
        ReactiveFormsModule
    ],
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.css'
})
export class LoginFormComponent {
  returnUrl: string | undefined = undefined;

  formLogin: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email, Validators.maxLength(64)]),
    password: new FormControl('', [Validators.required, Validators.minLength(8), Validators.maxLength(64)])
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
    }
  };

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      if (params['returnUrl']) {
        this.returnUrl = params['returnUrl'];
      }
    });
  }

  constructor(private route: ActivatedRoute,
              private router: Router,
              private userHttpService: UserHttpService,
              private sessionService: SessionService,
              private themeService: ThemeService) { }

  getErrorMessage(controlName: string): string {
    const control = this.formLogin.get(controlName);
    if (control?.errors) {
      for (const error in control.errors) {
        if (this.errorMessages[controlName][error]) {
          return this.errorMessages[controlName][error];
        }
      }
    }
    return '';
  }

  onSubmitLogin(): void {
    if (this.formLogin.invalid) {
      this.formLogin.markAllAsTouched();
      return;
    }

    const body: LoginUser = {
      email: this.formLogin.value.email,
      password: this.formLogin.value.password,
    };

    this.userHttpService.login(body).subscribe({
      next: (result: UserAuth): void => {
        this.sessionService.setSessionData({
          accessToken: result.accessToken,
          refreshToken: result.refreshToken,
          email: result.user.email,
          username: result.user.username,
          theme: result.user.theme,
          avatarBackground: result.user.avatarBackground
        });
        this.themeService.applyTheme(result.user.theme);
        if (this.returnUrl != undefined) {
          this.router.navigate([this.returnUrl]);
        } else {
          this.router.navigate(['/dashboard']);
        }      }
    });
  }
}
