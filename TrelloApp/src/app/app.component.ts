import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {AuthComponent} from './articles/auth/auth.component';
import {BoardDashboardComponent} from './articles/board-dashboard/board-dashboard.component';
import {BoardViewComponent} from './articles/board-view/board-view.component';
import {BoardMenuComponent} from './articles/board-menu/board-menu.component';
import {TaskViewComponent} from './articles/task-view/task-view.component';

@Component({
  selector: 'app-root',
  imports: [BoardViewComponent, BoardMenuComponent, BoardDashboardComponent, AuthComponent, TaskViewComponent, RouterOutlet],
  templateUrl: './app.component.html',
  standalone: true,
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'TrelloApp';
}
