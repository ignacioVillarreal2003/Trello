import {Component, Input} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {UserListComponent} from '../user-list/user-list.component';
import {Card} from '../../../core/models/card';
import {InputComponent} from "../../../shared/components/input/input.component";

@Component({
  selector: 'app-card-users',
    imports: [
        ReactiveFormsModule,
        UserListComponent,
        FormsModule,
        InputComponent
    ],
  templateUrl: './card-users.component.html',
  styleUrl: './card-users.component.css'
})
export class CardUsersComponent {
  @Input() card: Card | undefined = undefined;
  @Input() boardId: number | undefined = undefined;
  searchTerm: string = '';
}
