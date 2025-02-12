export interface List {
  id?: string,
  title?: string,
  description?: string
  listId?: string
}

export interface AddList {
  title: string,
  description: string
}

export interface UpdateList {
  title?: string,
  description?: string}
