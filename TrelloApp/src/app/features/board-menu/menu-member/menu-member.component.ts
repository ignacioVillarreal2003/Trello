import {Component, Input} from '@angular/core';
import {Board} from '../../../core/models/board';
import {MemberAddFormComponent} from '../member-add-form/member-add-form.component';
import {MemberListComponent} from '../member-list/member-list.component';

@Component({
  selector: 'app-menu-member',
  imports: [
    MemberAddFormComponent,
    MemberListComponent
  ],
  templateUrl: './menu-member.component.html',
  styleUrl: './menu-member.component.css'
})
export class MenuMemberComponent {
  @Input() board: Board | undefined = undefined;
}
