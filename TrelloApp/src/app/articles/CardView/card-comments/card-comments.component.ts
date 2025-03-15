import {Component, Input} from '@angular/core';
import {NgForOf, NgIf} from '@angular/common';
import {Card} from '../../../core/models/card';
import {CommentHttpService} from '../../../core/services/http/comment-http.service';
import {AddComment, Comment} from '../../../core/models/comment';
import {CommunicationService} from '../../../core/services/communication.service';
import {AlertService} from '../../../core/services/alert.service';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {BtnComponent} from '../../../shared/components/btn/btn.component';
import {TextareaComponent} from '../../../shared/components/textarea/textarea.component';

@Component({
  selector: 'app-card-comments',
  imports: [
    NgIf,
    NgForOf,
    BtnComponent,
    ReactiveFormsModule,
    TextareaComponent
  ],
  templateUrl: './card-comments.component.html',
  styleUrl: './card-comments.component.css'
})
export class CardCommentsComponent {
  @Input() card: Card | undefined = undefined;
  comments: Comment[] = [];

  constructor(private commentHttpService: CommentHttpService,
              private alertService: AlertService) {
  }

  ngOnInit() {
    this.getComments();
  }

  getComments(): void {
    if (this.card) {
      this.commentHttpService.getCommentsByCardId(this.card.id).subscribe({
        next: (result: Comment[]) => {
          this.comments = result;
        },
        error: (error: Error): void => {
          this.alertService.ErrorMessage(error.message);
        }
      })
    }
  }

  formAddComment: FormGroup = new FormGroup({
    text: new FormControl('', [Validators.required, Validators.maxLength(255)]),
  });

  addComment(): void {
    if (this.card) {
      if (this.formAddComment.invalid) {
        if (this.formAddComment.controls['text'].errors) {
          this.alertService.ErrorMessage('Invalid content.');
        }
      } else {
        const body: AddComment = {
          text: this.formAddComment.controls['text'].value,
        }
        this.commentHttpService.add(this.card.id, body).subscribe({
          next: (result: Comment): void => {
            this.comments.push(result);
          },
          error: (error: Error): void => {
            this.alertService.ErrorMessage(error.message);
          }
        })
      }
    }
  }
}
