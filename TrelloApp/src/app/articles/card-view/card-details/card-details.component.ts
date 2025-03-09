import { Component } from '@angular/core';
import {NgForOf, NgIf} from '@angular/common';

@Component({
  selector: 'app-card-details',
  imports: [
    NgIf,
    NgForOf
  ],
  templateUrl: './card-details.component.html',
  styleUrl: './card-details.component.css'
})
export class CardDetailsComponent {
  isDescriptionActive: boolean = false;

  users = [
    { id: 1, name: 'pablo', checked: false },
    { id: 2, name: 'chacon', checked: false },
    { id: 3, name: 'Julio', checked: false }
  ];

  labels = [
    { id: 1, name: 'Bug', checked: false, color: 'green' },
    { id: 2, name: 'Feature', checked: false, color: 'red' },
    { id: 3, name: 'Enhancement', checked: false, color: 'blue' }
  ];
}
