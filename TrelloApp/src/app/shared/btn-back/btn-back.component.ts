import {Component, Input} from '@angular/core';
import {NgClass, NgIf} from '@angular/common';

@Component({
  selector: 'app-btn-back',
  imports: [
    NgIf,
    NgClass
  ],
  templateUrl: './btn-back.component.html',
  styleUrl: './btn-back.component.css'
})
export class BtnBackComponent {
  @Input() rounded: 'r-small' | 'r-large' = 'r-small';
  @Input() size: 's-large' | 's-medium' | 's-small' = 's-medium';
}
