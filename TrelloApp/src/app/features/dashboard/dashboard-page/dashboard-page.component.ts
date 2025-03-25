import { Component } from '@angular/core';
import {BtnComponent} from "../../../shared/components/btn/btn.component";
import {HeaderComponent} from "../../header/header.component";
import {NgIf} from "@angular/common";
import {BoardListComponent} from '../board-list/board-list.component';
import {BoardCreateFormComponent} from '../board-create-form/board-create-form.component';

@Component({
  selector: 'app-dashboard-page',
  imports: [
    BtnComponent,
    HeaderComponent,
    NgIf,
    BoardListComponent,
    BoardCreateFormComponent
  ],
  templateUrl: './dashboard-page.component.html',
  styleUrl: './dashboard-page.component.css'
})
export class DashboardPageComponent {
  isBoardCreateFormOpen: boolean = false;

  openBoardCreateForm(): void {
    this.isBoardCreateFormOpen = true;
  }

  closeBoardCreateForm(): void {
    this.isBoardCreateFormOpen = false;
  }
}
