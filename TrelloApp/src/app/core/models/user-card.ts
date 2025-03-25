import {Card} from './card';
import {User} from './user';

export interface UserCard {
  cardId: number,
  userId: number,
  card: Card,
  user: User
}

export interface AddUserCard {
  userId: number,
}
