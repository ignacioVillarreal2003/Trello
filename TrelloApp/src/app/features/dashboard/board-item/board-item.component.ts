import {Component, Input} from '@angular/core';
import {Board} from '../../../core/models/board';
import {NgIf} from '@angular/common';
import {Router} from '@angular/router';
import {ResourcesService} from '../../../core/services/resources.service';
import {BoardCookieService} from '../../../core/services/session/board-cookie.service';

@Component({
  selector: 'app-board-item',
  imports: [
    NgIf
  ],
  templateUrl: './board-item.component.html',
  styleUrl: './board-item.component.css'
})
export class BoardItemComponent {
  @Input() board: Board | undefined = undefined;
  boardBackgroundPath: string | undefined = undefined;

  constructor(private router: Router,
              private boardCookieService: BoardCookieService,
              private resourcesService: ResourcesService) {}

  ngOnInit(): void {
    this.boardBackgroundPath = this.resourcesService.boardBackgroundPath;
  }

  openBoard(): void {
    if (this.board !== undefined) {
      this.boardCookieService.setCookie(this.board.id)
      this.router.navigate([`/board/${this.board.id}`]);
    }
  }
}
