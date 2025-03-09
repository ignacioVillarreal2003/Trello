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
  @Input() size: string = '32px';
}
