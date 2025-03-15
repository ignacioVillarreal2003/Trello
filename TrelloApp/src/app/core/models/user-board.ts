export interface UserBoard {
  boardId: number,
  userId: number,
  role: string
}

export interface AddUserBoard {
  userId: number,
  role: string
}
