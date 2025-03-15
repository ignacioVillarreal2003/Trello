import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-btn-delete',
    imports: [],
  templateUrl: './btn-delete.component.html',
  styleUrl: './btn-delete.component.css'
})
export class BtnDeleteComponent {
  @Input() size: string = '32px';
}
