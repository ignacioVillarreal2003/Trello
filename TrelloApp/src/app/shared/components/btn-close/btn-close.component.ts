import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-btn-close',
  imports: [
  ],
  templateUrl: './btn-close.component.html',
  styleUrl: './btn-close.component.css'
})
export class BtnCloseComponent {
  @Input() size: string = '32px';
}
