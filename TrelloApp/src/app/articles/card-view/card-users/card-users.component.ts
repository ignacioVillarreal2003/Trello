import {Component, EventEmitter, Output} from '@angular/core';
import {NgForOf, NgIf} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

@Component({
  selector: 'app-card-users',
  imports: [
    NgForOf,
    NgIf,
    ReactiveFormsModule,
    FormsModule
  ],
  templateUrl: './card-users.component.html',
  styleUrl: './card-users.component.css'
})
export class CardUsersComponent {
  searchTerm = '';

  users = [
    { id: 1, name: 'Ignacio Villarreal', checked: false },
    { id: 2, name: 'John Doe', checked: false },
  ];

  get filteredUsers() {
    return this.users.filter(user =>
      user.name.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }
}
