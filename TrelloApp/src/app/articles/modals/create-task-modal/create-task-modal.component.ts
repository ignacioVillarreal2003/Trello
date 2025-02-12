import {Component, EventEmitter, Input, Output} from '@angular/core';
import {AlertService} from '../../../core/services/alert.service';
import {TaskHttpService} from '../../../core/services/http/task-http.service';
import {CommunicationService} from '../../../core/services/communication.service';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';

@Component({
  selector: 'app-create-task-modal',
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './create-task-modal.component.html',
  standalone: true,
  styleUrl: './create-task-modal.component.css'
})
export class CreateTaskModalComponent {
  @Input() listId: number | undefined = undefined;
  @Output() close: EventEmitter<void> = new EventEmitter<void>();

  constructor(private alertService: AlertService,
              private taskHttpService: TaskHttpService,
              private communicationService: CommunicationService) {}

  createTaskForm: FormGroup = new FormGroup({
    title: new FormControl('', [Validators.required]),
    description: new FormControl('', [Validators.required])
  });

  onSubmitCreateTask(): void {
    if (this.createTaskForm.invalid) {
      if (this.createTaskForm.controls["title"].errors) {
        this.alertService.ErrorMessage('Title is required.');
      }
      if (this.createTaskForm.controls["description"].errors) {
        this.alertService.ErrorMessage('Description is required.');
      }
      return;
    }
    if (this.listId) {
      this.taskHttpService.postTask(this.createTaskForm.value.title, this.createTaskForm.value.description, this.listId).subscribe({
        next: (response: any): void => {
          this.alertService.SuccessMessage('Successfully created list.ts.');
          this.communicationService.triggerRefreshTasks();
        },
        error: (error: any): void => {
          const errorMessage: string = error?.message || 'Error in the server. Try again later.';
          this.alertService.ErrorMessage(errorMessage);
        }
      });
    }
  }

  onClose(): void {
    this.close.emit();
  }
}
