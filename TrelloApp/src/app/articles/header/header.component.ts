import {Component, Input} from '@angular/core';
import {RouterLink} from '@angular/router';
import {SessionServiceService} from '../../core/services/session/session-service.service';
import {Subscription} from 'rxjs';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-header',
  imports: [
    RouterLink,
    NgIf,
  ],
  templateUrl: './header.component.html',
  standalone: true,
  styleUrl: './header.component.css'
})
export class HeaderComponent {

  constructor(private sessionServiceService: SessionServiceService) {}

  ngOnInit(): void {
    this.username = this.sessionServiceService.getSessionData()?.username;
  }

  /* Username */
  username: string | undefined = undefined;
}
