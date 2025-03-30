import {Card} from './card';
import {User} from './user';

export interface UserCard {
  id: number,
  createdAt: string,
  updatedAt?: string,
  cardId: number,
  userId: number,
  card: Card,
  user: User
}

export interface AddUserCard {
  userId: number,
}
