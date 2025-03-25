import { Component } from '@angular/core';
import {NgIf} from "@angular/common";
import {ReactiveFormsModule} from "@angular/forms";
import {RouterLink, RouterOutlet} from '@angular/router';

@Component({
  selector: 'app-auth-page',
  imports: [
    NgIf,
    ReactiveFormsModule,
    RouterOutlet,
    RouterLink
  ],
  templateUrl: './auth-page.component.html',
  styleUrl: './auth-page.component.css'
})
export class AuthPageComponent {
  currentMode: string = 'login';

  setMode(mode: string) {
    this.currentMode = mode;
  }
}
