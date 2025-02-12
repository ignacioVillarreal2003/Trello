export interface List {
  id?: string,
  title?: string,
  boardId?: string
}

export interface AddList {
  title: string,
}

export interface UpdateList {
  title: string
}
