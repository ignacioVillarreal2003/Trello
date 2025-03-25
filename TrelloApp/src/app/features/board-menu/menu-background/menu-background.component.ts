import {Component, Input} from '@angular/core';
import {Board} from '../../../core/models/board';
import {BackgroundListComponent} from '../background-list/background-list.component';

@Component({
  selector: 'app-menu-background',
  imports: [
    BackgroundListComponent
  ],
  templateUrl: './menu-background.component.html',
  styleUrl: './menu-background.component.css'
})
export class MenuBackgroundComponent {
  @Input() board: Board | undefined = undefined;
}
