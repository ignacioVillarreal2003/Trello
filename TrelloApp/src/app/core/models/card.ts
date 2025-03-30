import {List} from './list';
import {CardLabel} from './card-label';

export interface Card {
  id: number,
  title: string,
  description: string,
  listId: number,
  isCompleted: boolean,
  createdAt: string,
  updatedAt?: string,
  position: number,
  list: List,
  comments: Comment[],
  cardLabels: CardLabel[]
}

export interface AddCard {
  title: string,
  description: string,
  position: number
}

export interface UpdateCard {
  title?: string,
  description?: string,
  listId?: number,
  isCompleted?: boolean,
  position?: number
}
