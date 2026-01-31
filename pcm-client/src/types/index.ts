// src/types/index.ts

export interface User {
  id: number;
  username: string;
  email: string;
}

export interface Member {
  id: number;
  name: string;
  email: string;
  phone: string;
  position: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface ApiResponse<T = any> {
  success: boolean;
  data: T;
  message?: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user?: User;
}
