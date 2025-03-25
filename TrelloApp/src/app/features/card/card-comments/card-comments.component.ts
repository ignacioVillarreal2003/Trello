import {Component, Input} from '@angular/core';
import {Card} from '../../../core/models/card';
import {ReactiveFormsModule} from '@angular/forms';
import {CommentCreateFormComponent} from '../comment-create-form/comment-create-form.component';
import {CommentListComponent} from '../comment-list/comment-list.component';

@Component({
  selector: 'app-card-comments',
  imports: [
    ReactiveFormsModule,
    CommentCreateFormComponent,
    CommentListComponent
  ],
  templateUrl: './card-comments.component.html',
  styleUrl: './card-comments.component.css'
})
export class CardCommentsComponent {
  @Input() card: Card | undefined = undefined;
}
