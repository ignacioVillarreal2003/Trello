import {User} from './user';
import {Board} from './board';

export interface UserBoard {
  boardId: number,
  userId: number,
  role: string
  user: User,
  board: Board
}

export interface AddUserBoard {
  userId: number,
  role: string
}
