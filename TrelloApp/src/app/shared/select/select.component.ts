import {Component, EventEmitter, Input, Output} from '@angular/core';
import {NgClass, NgForOf, NgIf} from "@angular/common";
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-select',
  imports: [
    NgIf,
    FormsModule,
    NgClass,
    NgForOf
  ],
  templateUrl: './select.component.html',
  styleUrl: './select.component.css'
})
export class SelectComponent {
  @Input() options: { value: any, label: string }[] = [];
  @Input() selectedValue: any;
  @Output() selectionChange = new EventEmitter<any>();
  id: string = `input-${Math.random().toString(36).substr(2, 9)}`;
  @Input() appearance: 'appearance-solid' | 'appearance-outline' = 'appearance-solid';
  @Input() rounded: 'r-small' | 'r-medium' | 'r-large' = 'r-small';
  @Input() color: 'red' | 'blue' | 'skyblue' | 'orange' | 'grey' | 'yellow' | 'green' = 'blue';
  @Input() size: 's-tiny' | 's-small' | 's-medium' | 's-large' = 's-medium';
  @Input() textLabel: string = '';

  onSelectChange(value: any) {
    this.selectionChange.emit(value);
  }
}
