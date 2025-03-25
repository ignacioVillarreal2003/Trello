import {Component, EventEmitter, Input, Output} from '@angular/core';
import {BtnComponent} from "../../../shared/components/btn/btn.component";
import {InputComponent} from "../../../shared/components/input/input.component";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {ListHttpService} from '../../../core/services/http/list-http.service';
import {AddList} from '../../../core/models/list';

@Component({
  selector: 'app-list-create-form',
    imports: [
        BtnComponent,
        InputComponent,
        ReactiveFormsModule
    ],
  templateUrl: './list-create-form.component.html',
  styleUrl: './list-create-form.component.css'
})
export class ListCreateFormComponent {
  @Input() boardId: number | undefined = undefined;
  @Input() listsCount: number | undefined = undefined;
  @Output() close: EventEmitter<void> = new EventEmitter<void>();

  formCreateList: FormGroup = new FormGroup({
    title: new FormControl('', [Validators.required, Validators.maxLength(64)]),
  });

  errorMessages: any = {
    title: {
      required: 'Title is required.',
      maxlength: 'Title must be less than 64 characters.'
    }
  };

  constructor(private listHttpService: ListHttpService) {}

  getErrorMessage(controlName: string): string {
    const control = this.formCreateList.get(controlName);
    if (control?.errors) {
      for (const error in control.errors) {
        if (this.errorMessages[controlName][error]) {
          return this.errorMessages[controlName][error];
        }
      }
    }
    return '';
  }

  onSubmitCreateList(): void {
    if (this.formCreateList.invalid) {
      this.formCreateList.markAllAsTouched();
      return;
    }

    if (this.boardId != undefined && this.listsCount != undefined) {
      const body: AddList = {
        title: this.formCreateList.value.title,
        position: this.listsCount
      }

      this.listHttpService.add(this.boardId, body).subscribe({
        next: (): void => {
          this.onClose();
        }
      });
    }
  }

  onClose(): void {
    this.close.emit();
  }
}
