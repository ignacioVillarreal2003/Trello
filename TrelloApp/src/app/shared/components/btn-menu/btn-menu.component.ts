import {Component, Input} from '@angular/core';
import {SessionService} from '../../../core/services/session/session.service';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-btn-menu',
  imports: [
    NgIf
  ],
  templateUrl: './btn-menu.component.html',
  styleUrl: './btn-menu.component.css'
})
export class BtnMenuComponent {
  @Input() size: string = '32px';
  @Input() theme: string | undefined = undefined;

  constructor(private sessionService: SessionService) {
  }

  ngOnInit() {
    this.theme = this.sessionService.getSessionData()?.theme;
  }
}
