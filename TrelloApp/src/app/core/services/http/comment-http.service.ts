import { Injectable } from '@angular/core';
import {catchError, Observable} from 'rxjs';
import {BaseHttpService} from './base-http.service';
import {HttpClient} from '@angular/common/http';
import {AddComment, Comment, UpdateComment} from '../../models/comment';

@Injectable({
  providedIn: 'root'
})
export class CommentHttpService extends BaseHttpService {

  url = `${this.baseUrl}/Comment`;

  constructor(http: HttpClient) {
    super(http);
  }

  getCommentById(commentId: number): Observable<Comment> {
    return this.http.get<Comment>(`${this.url}/${commentId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  getCommentsByCardId(cardId: number): Observable<Comment[]> {
    return this.http.get<Comment[]>(`${this.url}/card/${cardId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  add(cardId: number, requestBody: AddComment): Observable<Comment> {
    return this.http.post<Comment>(`${this.url}/card/${cardId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  update(commentId: number, requestBody: UpdateComment): Observable<Comment> {
    return this.http.put<Comment>(`${this.url}/${commentId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  delete(commentId: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${commentId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
