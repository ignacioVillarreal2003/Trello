import {Component, Input} from '@angular/core';
import {RouterLink} from '@angular/router';
import {SessionService} from '../../core/services/session/session.service';
import {Subscription} from 'rxjs';
import {NgIf} from '@angular/common';
import {AvatarComponent} from '../../shared/components/avatar/avatar.component';

@Component({
  selector: 'app-header',
  imports: [
    RouterLink,
    NgIf,
    AvatarComponent,
  ],
  templateUrl: './header.component.html',
  standalone: true,
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  username: string | undefined = undefined;
  avatarBackground: string | undefined = undefined;

  constructor(private sessionServiceService: SessionService) {}

  ngOnInit(): void {
    this.username = this.sessionServiceService.getSessionData()?.username;
    this.avatarBackground = this.sessionServiceService.getSessionData()?.avatarBackground
  }
}
