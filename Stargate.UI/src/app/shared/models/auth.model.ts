import { BaseResponse } from './base-response.model';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse extends BaseResponse {
  token?: string;
  username?: string;
}
