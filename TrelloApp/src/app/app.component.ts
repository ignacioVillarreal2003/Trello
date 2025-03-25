import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {SessionService} from './core/services/session/session.service';
import {ThemeService} from './core/services/theme.service';
import {UserCommunicationService} from './core/services/communication/user-communication.service';
import {User} from './core/models/user';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  standalone: true,
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'TrelloApp';

  constructor(private sessionService: SessionService,
              private themeService: ThemeService,
              private userCommunicationService: UserCommunicationService) {}

  ngOnInit(): void {
    const theme: string | undefined = this.sessionService.getSessionData()?.theme;
    if (theme != undefined) {
      this.themeService.applyTheme(theme);
    }
    this.userCommunicationService.updateUser$.subscribe((user: User | null): void => {
      if (user != null) {
        this.themeService.applyTheme(user.theme);
      }
    })
  }
}
