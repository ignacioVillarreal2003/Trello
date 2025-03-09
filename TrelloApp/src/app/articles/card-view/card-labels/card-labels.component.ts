import { Component } from '@angular/core';
import {NgForOf, NgIf} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {CheckboxComponent} from '../../../shared/components/checkbox/checkbox.component';

@Component({
  selector: 'app-card-labels',
  imports: [
    NgForOf,
    NgIf,
    ReactiveFormsModule,
    FormsModule,
    CheckboxComponent
  ],
  templateUrl: './card-labels.component.html',
  styleUrl: './card-labels.component.css'
})
export class CardLabelsComponent {
  searchTerm = '';

  labels = [
    { id: 1, name: 'Ignacio Villarreal', checked: false, color: '#4BCE97' },
    { id: 2, name: 'John Doe', checked: false, color: '#F5CD47' },
    { id: 3, name: 'John Doe 2', checked: false, color: '#FEA362' },
    { id: 3, name: '', checked: false, color: '#FEA362' },
    { id: 1, name: 'Ignacio Villarreal', checked: false, color: '#4BCE97' },
    { id: 2, name: 'John Doe', checked: false, color: '#F5CD47' },
    { id: 3, name: 'John Doe 2', checked: false, color: '#FEA362' },
    { id: 3, name: '', checked: false, color: '#FEA362' },
    { id: 1, name: 'Ignacio Villarreal', checked: false, color: '#4BCE97' },
    { id: 2, name: 'John Doe', checked: false, color: '#F5CD47' },
    { id: 3, name: 'John Doe 2', checked: false, color: '#FEA362' },
    { id: 3, name: '', checked: false, color: '#FEA362' },
    { id: 1, name: 'Ignacio Villarreal', checked: false, color: '#4BCE97' },
    { id: 2, name: 'John Doe', checked: false, color: '#F5CD47' },
    { id: 3, name: 'John Doe 2', checked: false, color: '#FEA362' },
    { id: 3, name: '', checked: false, color: '#FEA362' },
    { id: 1, name: 'Ignacio Villarreal', checked: false, color: '#4BCE97' },
    { id: 2, name: 'John Doe', checked: false, color: '#F5CD47' },
    { id: 3, name: 'John Doe 2', checked: false, color: '#FEA362' },
    { id: 3, name: '', checked: false, color: '#FEA362' },
  ];

  get filteredLabels() {
    return this.labels.filter(label =>
      label.name.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }
}
