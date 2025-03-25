import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {

  applyTheme(theme: string): void {
    const body: HTMLElement = document.querySelector('body') as HTMLElement;
    body.classList.remove('dark', 'light');
    body.classList.add(theme ? theme.toLowerCase() : 'light');
  }
}
