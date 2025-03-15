import {User} from './user';

export interface Label {
  id: number,
  title: string,
  color: string,
  boardId: number,
  createdAt: string,
  updatedAt: string
}

export interface LabelWithAssignment extends Label {
  isAssigned: boolean;
}

export interface AddLabel {
  title: string;
  color: string;
}

export interface UpdateLabel {
  title?: string;
  color?: string;
}
