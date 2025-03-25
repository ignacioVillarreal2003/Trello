import {Component, Input} from '@angular/core';
import {Router} from '@angular/router';
import {Card} from '../../../core/models/card';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-card-item',
  imports: [
    NgIf
  ],
  templateUrl: './card-item.component.html',
  styleUrl: './card-item.component.css'
})
export class CardItemComponent {
  @Input() card: Card | undefined = undefined;

  constructor(private router: Router) {}

  openCard(id: number) {
    this.router.navigate([`card/${id}`])
  }
}
