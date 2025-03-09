export interface User {
  id: number,
  email: string,
  username: string,
  theme: string,
  createdAt: string,
  updatedAt?: string,
}

export interface UserAuth {
  user: User,
  accessToken: string,
  refreshToken: string
}

export interface RegisterUser {
  email: string;
  password: string;
  username: string;
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
