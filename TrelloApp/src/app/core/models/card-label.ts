import {Card} from './card';
import {Label} from './label';

export interface CardLabel {
  cardId: number,
  labelId: number,
  card: Card,
  label: Label
}

export interface AddCardLabel {
  labelId: number,
}
