import {Component, Input} from '@angular/core';
import {NgIf} from "@angular/common";
import {SessionService} from '../../../core/services/session/session.service';

@Component({
  selector: 'app-btn-theme',
    imports: [
        NgIf
    ],
  templateUrl: './btn-theme.component.html',
  styleUrl: './btn-theme.component.css'
})
export class BtnThemeComponent {
  @Input() theme: string | undefined = undefined;

  constructor(private sessionService: SessionService) {
  }

  ngOnInit() {
    this.theme = this.sessionService.getSessionData()?.theme;
  }
}
