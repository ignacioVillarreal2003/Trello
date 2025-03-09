import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-btn-menu',
  imports: [],
  templateUrl: './btn-menu.component.html',
  styleUrl: './btn-menu.component.css'
})
export class BtnMenuComponent {
  @Input() size: string = '32px';
}
