import { Component } from '@angular/core';
import {BtnComponent} from "../../../shared/components/btn/btn.component";
import {InputComponent} from "../../../shared/components/input/input.component";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {AlertService} from '../../../core/services/alert.service';
import {UserHttpService} from '../../../core/services/http/user-http.service';
import {UpdateUser} from '../../../core/models/user';

@Component({
  selector: 'app-change-password-form',
    imports: [
        BtnComponent,
        InputComponent,
        ReactiveFormsModule
    ],
  templateUrl: './change-password-form.component.html',
  styleUrl: './change-password-form.component.css'
})
export class ChangePasswordFormComponent {

  formUpdatePassword: FormGroup = new FormGroup({
    oldPassword: new FormControl('', [Validators.required, Validators.minLength(8), Validators.maxLength(64)]),
    newPassword: new FormControl('', [Validators.required, Validators.minLength(8), Validators.maxLength(64)])
  })

  errorMessages: any = {
    oldPassword: {
      required: 'Old password is required.',
      minlength: 'The password must be at least 8 characters long.',
      maxlength: 'The password must be less than 64 characters.'
    },
    newPassword: {
      required: 'New password is required.',
      minlength: 'The password must be at least 8 characters long.',
      maxlength: 'The password must be less than 64 characters.'
    }
  };

  constructor(private alertService: AlertService,
              private userHttpService: UserHttpService) { }

  getErrorMessage(controlName: string): string {
    const control = this.formUpdatePassword.get(controlName);
    if (control?.errors) {
      for (const error in control.errors) {
        if (this.errorMessages[controlName][error]) {
          return this.errorMessages[controlName][error];
        }
      }
    }
    return '';
  }

  onSubmitUpdatePassword(): void {
    if (this.formUpdatePassword.invalid) {
      this.formUpdatePassword.markAllAsTouched();
      return;
    }

    const body: UpdateUser = {
      oldPassword: this.formUpdatePassword.value.oldPassword,
      newPassword: this.formUpdatePassword.value.newPassword
    }

    this.userHttpService.update(body).subscribe({
      next: (result: any): void => {
        this.alertService.SuccessMessage("Password changed successfully.");
      }
    });
  }
}
