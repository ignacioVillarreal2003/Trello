export interface Comment {
  id: number,
  text: string
  cardId: number,
  authorId: number,
  createdAt: string,
  updatedAt: string
}

export interface AddComment {
  text: string;
}

export interface UpdateComment {
  text: string;
}
