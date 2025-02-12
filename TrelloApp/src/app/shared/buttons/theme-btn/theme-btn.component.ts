import {Component, ElementRef} from '@angular/core';
import {Subscription} from 'rxjs';
import {SessionServiceService} from '../../../core/services/session-service.service';
import {HttpErrorResponse} from '@angular/common/http';
import {UserHttpService} from '../../../core/services/http/user-http.service';
import {AlertService} from '../../../core/services/alert.service';

@Component({
  selector: 'app-theme-btn',
  imports: [],
  templateUrl: './theme-btn.component.html',
  standalone: true,
  styleUrl: './theme-btn.component.css'
})
export class ThemeBtnComponent {
  private subscription!: Subscription;

  constructor(private elRef: ElementRef,
              private sessionServiceService: SessionServiceService,
              private userHttpService: UserHttpService,
              private alertService: AlertService) {}

  ngOnInit(): void {
    this.subscription = this.sessionServiceService.getTheme().subscribe((theme: string) => {
      const inputElement: HTMLInputElement = this.elRef.nativeElement.querySelector('#themeToggle') as HTMLInputElement;
      inputElement.checked = theme === 'dark';
      const body: HTMLElement = document.querySelector('body') as HTMLElement;
      body.classList.toggle('dark', inputElement.checked);
      body.classList.toggle('light', !inputElement.checked);
    });
  }

  onSubmitUpdateTheme(event: Event): void {
    const inputElement: HTMLInputElement = event.target as HTMLInputElement;
    const isChecked: boolean = inputElement.checked;
    const theme: string = isChecked ? 'dark' : 'light';
    this.userHttpService.updateTheme(theme).subscribe({
      next: (response: any): void => {
        this.sessionServiceService.setUserData({
          email: response.user.email,
          username: response.user.username,
          theme: response.user.theme
        });
      },
      error: (error: HttpErrorResponse): void => {
        const errorMessage: string = error?.message || 'Error in the server. Try again later.';
        this.alertService.ErrorMessage(errorMessage);
      }
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
