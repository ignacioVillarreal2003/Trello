import { Injectable } from '@angular/core';
import {BaseHttpService} from './base-http.service';
import {HttpClient} from '@angular/common/http';
import {catchError, Observable} from 'rxjs';
import {Label} from '../../models/label';
import {AddCardLabel, CardLabel} from '../../models/card-label';

@Injectable({
  providedIn: 'root'
})
export class CardLabelHttpService extends BaseHttpService {
  url = `${this.baseUrl}/CardLabel`;

  constructor(http: HttpClient) {
    super(http);
  }

  getLabelsByCardId(cardId: number): Observable<Label[]> {
    return this.http.get<Label[]>(`${this.url}/card/${cardId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  addLabelToCard(cardId: number, requestBody: AddCardLabel): Observable<CardLabel> {
    return this.http.post<CardLabel>(`${this.url}/card/${cardId}`, requestBody, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }

  removeLabelFromCard(cardId: number, labelId: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/card/${cardId}/label/${labelId}`, this.httpOptions).pipe(
      catchError(this.handleError)
    );
  }
}
