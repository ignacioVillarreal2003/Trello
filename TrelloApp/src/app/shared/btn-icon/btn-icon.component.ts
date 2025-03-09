import {Component, Input} from '@angular/core';
import {NgClass, NgIf} from "@angular/common";

@Component({
  selector: 'app-btn-icon',
  imports: [
    NgIf,
    NgClass
  ],
  templateUrl: './btn-icon.component.html',
  styleUrl: './btn-icon.component.css'
})
export class BtnIconComponent {
  @Input() rounded: 'r-small' | 'r-large' = 'r-small';
  @Input() size: 's-large' | 's-medium' | 's-small' = 's-medium';
  @Input() color: 'red' | 'grey' | 'transparent' = 'transparent';
  @Input() icon: 'delete' | 'close' | 'back' | 'menu-1' | 'menu-2' | 'send' | 'dark-theme' | 'light-theme' = 'delete';
  @Input() iconColor: '#000000' | '#ffffff' = '#ffffff';
}
