import { Injectable } from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {Comment} from '../../models/comment';

@Injectable({
  providedIn: 'root'
})
export class CommentCommunicationService {
  private commentAddSubject = new BehaviorSubject<Comment | null>(null);
  addComment$ = this.commentAddSubject.asObservable();

  setAddComment(comment: Comment) {
    this.commentAddSubject.next(comment);
  }

  private commentUpdateSubject = new BehaviorSubject<Comment | null>(null);
  updateComment$ = this.commentUpdateSubject.asObservable();

  setUpdateComment(comment: Comment) {
    this.commentUpdateSubject.next(comment);
  }

  private commentDeleteSubject = new BehaviorSubject<number | null>(null);
  deleteComment$ = this.commentDeleteSubject.asObservable();

  setDeleteComment(commentId: number) {
    this.commentDeleteSubject.next(commentId);
  }
}
