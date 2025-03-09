export interface Card {
  id: string,
  title: string,
  description: string,
  listId: string,
  dueDate: string,
  priority?: string,
  isComplete: boolean,
  createdAt: string,
  updatedAt?: string,
}

export interface AddCard {
  title: string,
  description: string,
  priority?: string,
}

export interface UpdateCard {
  title: string,
  description: string,
  listId: string,
  dueDate: string,
  priority?: string,
  isComplete: boolean,
}
