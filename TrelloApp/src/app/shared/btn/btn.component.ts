import {Component, Input} from '@angular/core';
import {NgClass} from '@angular/common';

@Component({
  selector: 'app-btn',
  imports: [
    NgClass
  ],
  templateUrl: './btn.component.html',
  styleUrl: './btn.component.css'
})
export class BtnComponent {
  @Input() text: string = 'Button';
  @Input() appearance: 'appearance-solid' | 'appearance-outline' | 'appearance-light' | 'appearance-bordered' | 'appearance-flat' | 'appearance-ghost' | 'appearance-shadow' = 'appearance-solid';
  @Input() rounded: 'r-small' | 'r-medium' | 'r-large' = 'r-small';
  @Input() color: 'red' | 'blue' | 'skyblue' | 'orange' | 'grey' | 'yellow' | 'green' = 'blue';
  @Input() size: 's-tiny' | 's-small' | 's-medium' | 's-large' = 's-medium';
  @Input() aspectFree: boolean = true;
}
