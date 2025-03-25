import { Injectable } from '@angular/core';
import {BaseHttpService} from './base-http.service';
import {HttpClient} from '@angular/common/http';
import {catchError, Observable} from 'rxjs';
import {AddLabel, Label, UpdateLabel} from '../../models/label';

@Injectable({
  providedIn: 'root'
})
export class LabelHttpService extends BaseHttpService {

  url = `${this.baseUrl}/Label`;

  constructor(http: HttpClient) {
    super(http);
  }

  getLabelById(labelId: number): Observable<Label> {
    return this.http.get<Label>(`${this.url}/${labelId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  getLabelsByBoardId(boardId: number): Observable<Label[]> {
    return this.http.get<Label[]>(`${this.url}/board/${boardId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  add(boardId: number, requestBody: AddLabel): Observable<Label> {
    return this.http.post<Label>(`${this.url}/board/${boardId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  update(labelId: number, requestBody: UpdateLabel): Observable<Label> {
    return this.http.put<Label>(`${this.url}/${labelId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  delete(labelId: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${labelId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
