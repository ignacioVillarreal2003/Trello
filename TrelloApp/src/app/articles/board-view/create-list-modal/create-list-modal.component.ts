import {Component, EventEmitter, Input, Output} from '@angular/core';
import {AlertService} from '../../../core/services/alert.service';
import {ListHttpService} from '../../../core/services/http/list-http.service';
import {CommunicationService} from '../../../core/services/communication.service';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {InputComponent} from '../../../shared/components/input/input.component';
import {BtnComponent} from '../../../shared/components/btn/btn.component';
import {UpdateUser} from '../../../core/models/user';
import {AddList} from '../../../core/models/list';

@Component({
  selector: 'app-create-list-modal',
  imports: [
    ReactiveFormsModule,
    InputComponent,
    BtnComponent
  ],
  templateUrl: './create-list-modal.component.html',
  standalone: true,
  styleUrl: './create-list-modal.component.css'
})
export class CreateListModalComponent {
  @Input() boardId: number | undefined = undefined;
  @Input() listsCount: number | undefined = undefined;
  @Output() close: EventEmitter<void> = new EventEmitter<void>();

  constructor(private alertService: AlertService,
              private listHttpService: ListHttpService,
              private communicationService: CommunicationService) {}

  createListForm: FormGroup = new FormGroup({
    title: new FormControl('', [Validators.required]),
  });

  onSubmitCreateList(): void {
    if (this.createListForm.invalid) {
      if (this.createListForm.controls["title"].errors) {
        this.alertService.ErrorMessage('Title is required.');
      }
      return;
    }
    if (this.boardId != undefined && this.listsCount != undefined) {
      const body: AddList = {
        title: this.createListForm.value.title,
        position: this.listsCount
      }
      this.listHttpService.add(this.boardId, body).subscribe({
        next: (response: any): void => {
          this.alertService.SuccessMessage('Successfully created list.');
          this.communicationService.triggerRefreshLists();
          this.onClose();
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message);
        }
      });
    }
  }

  onClose(): void {
    this.close.emit();
  }
}
