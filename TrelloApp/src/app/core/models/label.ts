import {Board} from './board';
import {CardLabel} from './card-label';

export interface Label {
  id: number,
  title: string,
  color: string,
  boardId: number,
  createdAt: string,
  updatedAt?: string,
  board: Board,
  cardLabels: CardLabel[],
}

export interface LabelWithAssignment extends Label {
  isAssigned: boolean;
}

export interface AddLabel {
  title: string;
  color: string;
}

export interface UpdateLabel {
  title?: string;
  color?: string;
}
