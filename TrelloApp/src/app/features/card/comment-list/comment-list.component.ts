import {Component, Input} from '@angular/core';
import {Card} from '../../../core/models/card';
import {Comment} from '../../../core/models/comment';
import {CommentHttpService} from '../../../core/services/http/comment-http.service';
import {NgForOf} from '@angular/common';
import {CommentItemComponent} from '../comment-item/comment-item.component';
import {CommentCommunicationService} from '../../../core/services/communication/comment-communication.service';

@Component({
  selector: 'app-comment-list',
  imports: [
    NgForOf,
    CommentItemComponent
  ],
  templateUrl: './comment-list.component.html',
  styleUrl: './comment-list.component.css'
})
export class CommentListComponent {
  @Input() card: Card | undefined = undefined;
  comments: Comment[] = [];

  constructor(private commentHttpService: CommentHttpService,
              private commentCommunicationService: CommentCommunicationService) {
  }

  ngOnInit(): void {
    this.getComments();
    this.commentCommunicationService.addComment$.subscribe((comment: Comment | null) => {
      if (comment != null) {
        this.comments.push(comment);
      }
    });
    this.commentCommunicationService.updateComment$.subscribe((comment: Comment | null) => {
      if (comment != null) {
        this.comments = this.comments.map(c => c.id == comment.id ? comment : c);
      }
    });
    this.commentCommunicationService.deleteComment$.subscribe((commentId: number | null) => {
      if (commentId != null) {
        this.comments = this.comments.filter(c => c.id != commentId);
      }
    });
  }

  getComments(): void {
    if (this.card) {
      this.commentHttpService.getCommentsByCardId(this.card.id).subscribe({
        next: (result: Comment[]): void => {
          this.comments = result;
        }
      })
    }
  }
}
