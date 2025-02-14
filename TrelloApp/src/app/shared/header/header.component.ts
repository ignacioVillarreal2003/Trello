import {Component, Input} from '@angular/core';
import {RouterLink} from '@angular/router';
import {NgIf} from '@angular/common';
import {SessionServiceService} from '../../core/services/session-service.service';
import {UserMenuModalComponent} from '../../articles/modals/user-menu-modal/user-menu-modal.component';
import {Subscription} from 'rxjs';

@Component({
  selector: 'app-header',
  imports: [
    RouterLink,
    NgIf,
    UserMenuModalComponent
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

  switchUserMenu(): void {
    this.isOpenUserMenu = !this.isOpenUserMenu;
    const body: HTMLElement = document.querySelector('body') as HTMLElement;
    if (!this.isOpenUserMenu) {
      body.style.overflow = 'auto';
    } else {
      body.style.overflow = 'hidden';
    }
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
