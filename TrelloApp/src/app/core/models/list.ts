import { Task } from "./task"

export interface List {
  id?: string,
  title?: string,
  boardId?: string
  tasks: Task[]
}

export interface AddList {
  title: string,
}

export interface UpdateList {
  title: string
}
