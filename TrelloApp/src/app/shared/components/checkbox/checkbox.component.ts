import {Component, Input} from '@angular/core';
import {NgIf} from '@angular/common';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-checkbox',
  imports: [
    NgIf,
    FormsModule
  ],
  templateUrl: './checkbox.component.html',
  styleUrl: './checkbox.component.css'
})
export class CheckboxComponent {
  @Input() type: 1 | 2 = 1;
  @Input() checked: boolean = false;
}
