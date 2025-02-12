import {Component, ElementRef, Input} from '@angular/core';
import {NgIf} from '@angular/common';
import {Subscription} from 'rxjs';
import {SessionServiceService} from '../../../core/services/session-service.service';

@Component({
  selector: 'app-close-btn',
  imports: [
    NgIf
  ],
  templateUrl: './close-btn.component.html',
  standalone: true,
  styleUrl: './close-btn.component.css'
})
export class CloseBtnComponent {
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
