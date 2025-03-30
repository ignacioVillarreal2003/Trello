import {Card} from './card';
import {User} from './user';

export interface Comment {
  id: number,
  text: string
  cardId: number,
  authorId: number,
  createdAt: string,
  updatedAt?: string,
  card: Card,
  user: User
}

export interface AddComment {
  text: string;
}

export interface UpdateComment {
  text: string;
}
