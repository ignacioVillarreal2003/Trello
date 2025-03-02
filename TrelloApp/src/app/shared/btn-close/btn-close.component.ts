import {Component, Input} from '@angular/core';
import {NgClass, NgIf} from "@angular/common";

@Component({
  selector: 'app-btn-close',
  imports: [
    NgIf,
    NgClass
  ],
  templateUrl: './btn-close.component.html',
  styleUrl: './btn-close.component.css'
})
export class BtnCloseComponent {
  @Input() rounded: 'r-small' | 'r-large' = 'r-small';
  @Input() size: 's-large' | 's-medium' | 's-small' = 's-medium';
}
