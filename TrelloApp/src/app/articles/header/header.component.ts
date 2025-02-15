import {Component, Input} from '@angular/core';
import {RouterLink} from '@angular/router';
import {SessionServiceService} from '../../core/services/session-service.service';
import {Subscription} from 'rxjs';

@Component({
  selector: 'app-header',
  imports: [
    RouterLink,
  ],
  templateUrl: './header.component.html',
  standalone: true,
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  isOpenUserMenu: boolean = false;
  username: string = 'unnamed';
  private subscription!: Subscription;

  constructor(private sessionServiceService: SessionServiceService) {}

  ngOnInit(): void {
    this.subscription = this.sessionServiceService.getUsername().subscribe((username: string) => {
      this.username = username;
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
