import {Component, Input} from '@angular/core';
import {NgClass, NgIf} from "@angular/common";

@Component({
  selector: 'app-btn-menu',
  imports: [
    NgIf,
    NgClass
  ],
  templateUrl: './btn-menu.component.html',
  styleUrl: './btn-menu.component.css'
})
export class BtnMenuComponent {
  @Input() rounded: 'r-small' | 'r-large' = 'r-small';
  @Input() size: 's-large' | 's-medium' | 's-small' = 's-medium';
  @Input() icon: 'menu-1' | 'menu-2' = 'menu-2';
}
