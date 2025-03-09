export interface Board {
  id: number,
  title: string,
  description: string,
  background: string,
  createdAt: string,
  updatedAt?: string,
  isArchived: boolean,
  archivedAt?: string
}

export interface AddBoard {
  title: string;
  description?: string;
  background: string;
}

export interface UpdateBoard {
  title?: string;
  description?: string;
  background?: string;
  isArchived?: boolean,
}

