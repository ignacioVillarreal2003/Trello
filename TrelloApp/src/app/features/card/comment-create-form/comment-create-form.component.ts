import {Component, Input} from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {AddComment} from '../../../core/models/comment';
import {Card} from '../../../core/models/card';
import {CommentHttpService} from '../../../core/services/http/comment-http.service';
import {AlertService} from '../../../core/services/alert.service';
import {BtnComponent} from '../../../shared/components/btn/btn.component';
import {TextareaComponent} from '../../../shared/components/textarea/textarea.component';

@Component({
  selector: 'app-comment-create-form',
  imports: [
    BtnComponent,
    ReactiveFormsModule,
    TextareaComponent
  ],
  templateUrl: './comment-create-form.component.html',
  styleUrl: './comment-create-form.component.css'
})
export class CommentCreateFormComponent {
  @Input() card: Card | undefined = undefined;

  constructor(private commentHttpService: CommentHttpService,
              private alertService: AlertService) {
  }

  formAddComment: FormGroup = new FormGroup({
    text: new FormControl('', [Validators.required, Validators.maxLength(255)]),
  });

  addComment(): void {
    if (this.formAddComment.invalid) {
      if (this.formAddComment.controls['text'].errors) {
        this.alertService.ErrorMessage('Invalid content.');
      }
    }
    if (this.card) {
      const body: AddComment = {
        text: this.formAddComment.controls['text'].value,
      }
      this.commentHttpService.add(this.card.id, body).subscribe({
        next: (): void => {
          this.formAddComment.patchValue({
            text: ""
          });
        }
      })
    }
  }
}
