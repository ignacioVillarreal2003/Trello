export interface Comment {
  id?: number;
  text?: string;
  date?: string;
  taskId?: string;
}

export interface AddComment {
  text: string;
  taskId: string;
}

export interface UpdateComment {
  text: string;
}
