import {Component, Input} from '@angular/core';
import {NgForOf} from "@angular/common";
import {BackgroundItemComponent} from '../background-item/background-item.component';
import {Board} from '../../../core/models/board';
import {ResourcesService} from '../../../core/services/resources.service';
import {BoardHttpService} from '../../../core/services/http/board-http.service';
import {AlertService} from '../../../core/services/alert.service';

@Component({
  selector: 'app-background-list',
  imports: [
    NgForOf,
    BackgroundItemComponent
  ],
  templateUrl: './background-list.component.html',
  styleUrl: './background-list.component.css'
})
export class BackgroundListComponent {
  @Input() board: Board | undefined = undefined;
  backgrounds: string[] = []
  backgroundPath: string | undefined = undefined;

  constructor(private resourcesService: ResourcesService) {}

  ngOnInit(): void {
    this.backgrounds = this.resourcesService.boardBackgrounds;
    this.backgroundPath = this.resourcesService.boardBackgroundPath;
  }
}
