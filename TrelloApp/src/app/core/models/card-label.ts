import {Card} from './card';
import {Label} from './label';

export interface CardLabel {
  id: number,
  createdAt: string,
  updatedAt?: string,
  cardId: number,
  labelId: number,
  card: Card,
  label: Label
}

export interface AddCardLabel {
  labelId: number,
}
