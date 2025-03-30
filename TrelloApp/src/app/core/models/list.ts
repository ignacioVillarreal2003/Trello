import { Card } from "./card"
import {Board} from './board';

export interface List {
  id: number,
  title: string,
  position: number,
  boardId: number,
  createdAt: string,
  updatedAt?: string,
  board: Board,
  cards: Card[]
}

export interface AddList {
  title: string,
  position: number
}

export interface UpdateList {
  title?: string,
  position?: number
}
