export interface Task {
  id?: string,
  title?: string,
  description?: string
  listId?: string
}

export interface AddTask {
  title: string,
  description: string
}

export interface UpdateTask {
  title?: string,
  description?: string}
