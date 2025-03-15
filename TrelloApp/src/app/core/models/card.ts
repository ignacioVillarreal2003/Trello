export interface Card {
  id: number,
  title: string,
  description: string,
  listId: number,
  dueDate: string,
  priority?: string,
  isCompleted: boolean,
  createdAt: string,
  updatedAt?: string,
  position: number
}

export interface AddCard {
  title: string,
  description: string,
  priority?: string,
  position: number
}

export interface UpdateCard {
  title?: string,
  description?: string,
  listId?: number,
  dueDate?: string,
  priority?: string,
  isCompleted?: boolean,
  position?: number
}
