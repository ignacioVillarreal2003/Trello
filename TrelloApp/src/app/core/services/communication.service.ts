import { Injectable } from '@angular/core';
import {Observable, Subject} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CommunicationService {

  private refreshBoardsSubject: Subject<void> = new Subject<void>();
  refreshBoards$: Observable<any> = this.refreshBoardsSubject.asObservable();

  triggerRefreshBoards(): void {
    this.refreshBoardsSubject.next();
  }

  private refreshTasksSubject: Subject<void> = new Subject<void>();
  refreshTasks$: Observable<any> = this.refreshTasksSubject.asObservable();

  triggerRefreshTasks(): void {
    this.refreshTasksSubject.next();
  }

  private refreshListsSubject: Subject<void> = new Subject<void>();
  refreshLists$: Observable<any> = this.refreshListsSubject.asObservable();

  triggerRefreshLists(): void {
    this.refreshListsSubject.next();
  }

  private refreshBoardLabelsSubject: Subject<void> = new Subject<void>();
  refreshBoardLabels$: Observable<any> = this.refreshBoardLabelsSubject.asObservable();

  triggerRefreshBoardLabels(): void {
    this.refreshBoardLabelsSubject.next();
  }
}
