export interface Board {
  id?: number;
  icon?: string;
  theme?: string;
  title?: string;
}

export interface AddBoard {
  icon: string;
  theme: string;
  title: string;
}

export interface UpdateBoard {
  icon: string;
  theme: string;
  title: string;
}
