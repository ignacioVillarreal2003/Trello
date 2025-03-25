import {Component, Input} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {Board} from '../../../core/models/board';
import {LabelCreateFormComponent} from '../label-create-form/label-create-form.component';
import {LabelListComponent} from '../label-list/label-list.component';

@Component({
  selector: 'app-menu-label',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    LabelCreateFormComponent,
    LabelListComponent
  ],
  templateUrl: './menu-label.component.html',
  styleUrl: './menu-label.component.css'
})
export class MenuLabelComponent {
  @Input() board: Board | undefined = undefined;
}
