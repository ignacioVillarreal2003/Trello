import {User} from './user';
import {Board} from './board';

export interface UserBoard {
  id: number,
  createdAt: string,
  updatedAt?: string,
  boardId: number,
  userId: number,
  user: User,
  board: Board
}

export interface AddUserBoard {
  userId: number,
}
