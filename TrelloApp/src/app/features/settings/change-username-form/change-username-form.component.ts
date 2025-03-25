import { Component } from '@angular/core';
import {BtnComponent} from "../../../shared/components/btn/btn.component";
import {InputComponent} from "../../../shared/components/input/input.component";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {UserHttpService} from '../../../core/services/http/user-http.service';
import {SessionService} from '../../../core/services/session/session.service';
import {UpdateUser, User} from '../../../core/models/user';
import {UserCommunicationService} from '../../../core/services/communication/user-communication.service';

@Component({
  selector: 'app-change-username-form',
    imports: [
        BtnComponent,
        InputComponent,
        ReactiveFormsModule
    ],
  templateUrl: './change-username-form.component.html',
  styleUrl: './change-username-form.component.css'
})
export class ChangeUsernameFormComponent {

  formUpdateUsername: FormGroup = new FormGroup({
    username: new FormControl('', [Validators.required, Validators.maxLength(64)])
  })

  errorMessages: any = {
    username: {
      required: 'Username is required.',
      minlength: 'Username must be at least 8 characters long.',
      maxlength: 'Username must be less than 64 characters.'
    }
  };

  constructor(private userCommunicationService: UserCommunicationService,
              private userHttpService: UserHttpService,
              private sessionServiceService: SessionService) { }

  getErrorMessage(controlName: string): string {
    const control = this.formUpdateUsername.get(controlName);
    if (control?.errors) {
      for (const error in control.errors) {
        if (this.errorMessages[controlName][error]) {
          return this.errorMessages[controlName][error];
        }
      }
    }
    return '';
  }

  onSubmitUpdateUsername(): void {
    if (this.formUpdateUsername.invalid) {
      this.formUpdateUsername.markAllAsTouched();
      return;
    }

    const body: UpdateUser = {
      username: this.formUpdateUsername.value.username
    };

    this.userHttpService.update(body).subscribe({
      next: (result: User): void => {
        this.sessionServiceService.updateSessionData({ username: result.username });
        this.userCommunicationService.setUpdateUser(result);
      }
    });
  }
}
