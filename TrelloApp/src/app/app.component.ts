import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {CardViewComponent} from './articles/card-view/card-view.component';
import {AuthComponent} from './articles/auth/auth.component';
import {BoardDashboardComponent} from './articles/board-dashboard/board-dashboard.component';
import {BoardMenuComponent} from './articles/board-view/board-menu/board-menu.component';
import {UserMenuComponent} from './articles/user-menu/user-menu.component';
import {BoardViewComponent} from './articles/board-view/board-view.component';

@Component({
  selector: 'app-root',
  imports: [CardViewComponent, AuthComponent, BoardDashboardComponent, BoardMenuComponent, UserMenuComponent, BoardViewComponent, RouterOutlet],
  templateUrl: './app.component.html',
  standalone: true,
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'TrelloApp';
}
