import {Component, Input} from '@angular/core';
import {CdkDrag, CdkDragDrop, CdkDropList, CdkDropListGroup, moveItemInArray} from "@angular/cdk/drag-drop";
import {NgForOf, NgIf} from "@angular/common";
import {ListItemComponent} from '../list-item/list-item.component';
import {List, UpdateList} from '../../../core/models/list';
import {ListHttpService} from '../../../core/services/http/list-http.service';
import {ListCreateFormComponent} from '../list-create-form/list-create-form.component';
import {ListCommunicationService} from '../../../core/services/communication/list-communication.service';

@Component({
  selector: 'app-list-list',
  imports: [
    CdkDrag,
    CdkDropList,
    CdkDropListGroup,
    NgForOf,
    ListItemComponent,
    NgIf,
    ListCreateFormComponent
  ],
  templateUrl: './list-list.component.html',
  styleUrl: './list-list.component.css'
})
export class ListListComponent {
  @Input() boardId: number | undefined = undefined;
  lists: List[] = [];
  isListCreateFormOpen: boolean = false;

  constructor(private listHttpService: ListHttpService,
              private listCommunicationService: ListCommunicationService) {
  }

  ngOnInit(): void {
    this.getLists();
    this.listCommunicationService.addList$.subscribe((list: List | null): void => {
      console.log("lee", list)
      if (list !== null) {
        this.lists.push(list);
        this.lists.sort((a, b) => a.position - b.position);
      }
    });
    this.listCommunicationService.deleteList$.subscribe((listId: number | null): void => {
      if (listId !== null) {
        this.lists = this.lists.filter(l => l.id !== listId);
        this.lists.sort((a, b) => a.position - b.position);
      }
    });
    this.listCommunicationService.updateList$.subscribe((list: List | null): void => {
      if (list !== null) {
        this.lists = this.lists.map(l => l.id == list.id ? list : l);
        this.lists.sort((a, b) => a.position - b.position);
      }
    });
  }

  getLists(): void {
    if (this.boardId) {
      this.listHttpService.getListsByBoardId(this.boardId).subscribe({
        next: (result: List[]): void => {
          this.lists = result.sort((a, b) => a.position - b.position);
        }
      });
    }
  }

  openListCreateForm(): void {
    this.isListCreateFormOpen = true;
  }

  closeListCreateForm(): void {
    this.isListCreateFormOpen = false;
  }

  dropLists(event: CdkDragDrop<any[]>): void {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex)
    }
    this.onSubmitUpdateListsPositions();
  }

  onSubmitUpdateListsPositions(): void {
    this.lists.forEach((list: List, index: number): void => {
      const body: UpdateList = {
        position: index
      }

      this.listHttpService.update(list.id, body).subscribe();
    });
  }
}
