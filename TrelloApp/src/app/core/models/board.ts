import {Label} from './label';
import {UserBoard} from './user-board';
import {List} from './list';

export interface Board {
  id: number,
  title: string,
  background: string,
  createdAt: string,
  updatedAt?: string,
  lists: List[],
  userBoards: UserBoard[],
  labels: Label[]
}

export interface AddBoard {
  title: string;
  background: string;
}

export interface UpdateBoard {
  title?: string;
  background?: string;
}

