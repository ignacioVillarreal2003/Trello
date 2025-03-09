import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-btn-back',
  imports: [],
  templateUrl: './btn-back.component.html',
  styleUrl: './btn-back.component.css'
})
export class BtnBackComponent {
  @Input() size: string = '32px';
}
