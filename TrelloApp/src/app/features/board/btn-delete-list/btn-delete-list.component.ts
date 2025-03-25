import {Component, Input} from '@angular/core';
import {ListHttpService} from '../../../core/services/http/list-http.service';
import {ListCommunicationService} from '../../../core/services/communication/list-communication.service';
import {List} from '../../../core/models/list';

@Component({
  selector: 'app-btn-delete-list',
  imports: [],
  templateUrl: './btn-delete-list.component.html',
  styleUrl: './btn-delete-list.component.css'
})
export class BtnDeleteListComponent {
  @Input() list: List | undefined = undefined;

  constructor(private listHttpService: ListHttpService) {
  }

  onDelete(): void {
    if (this.list !== undefined) {
      this.listHttpService.delete(this.list.id).subscribe()
    }
  }
}
