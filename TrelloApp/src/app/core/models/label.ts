export interface Label {
  id?: number;
  title?: string;
  color?: string;
  taskId?: string;
}

export interface AddLabel {
  title: string;
  color: string;
}

export interface UpdateLabel {
  title?: string;
  color?: string;
}
