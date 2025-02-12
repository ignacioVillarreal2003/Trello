export interface User {
  id?: number;
  email?: string;
  password?: string;
  username?: string;
  token?: string;
  theme?: string
}

export interface RegisterUser {
  email: string;
  password: string;
  username: string;
  theme: string
}

export interface LoginUser {
  email: string;
  password: string;
}

export interface UpdateUser {
  username?: string;
  oldPassword?: string,
  newPassword?: string,
  theme?: string
}
