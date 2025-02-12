import { Component } from '@angular/core';
import {NgIf} from '@angular/common';
import {Subscription} from 'rxjs';
import {SessionServiceService} from '../../../core/services/session-service.service';

@Component({
  selector: 'app-back-btn',
  imports: [
    NgIf
  ],
  templateUrl: './back-btn.component.html',
  standalone: true,
  styleUrl: './back-btn.component.css'
})
export class BackBtnComponent {
  theme: string = 'light';

  private subscription!: Subscription;

  constructor(private sessionServiceService: SessionServiceService) {}

  ngOnInit(): void {
    this.subscription = this.sessionServiceService.getTheme().subscribe((theme: string) => {
      this.theme = theme;
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
