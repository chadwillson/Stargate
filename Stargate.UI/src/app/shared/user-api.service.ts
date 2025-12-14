import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { BaseResponse } from './models';

export interface UserRequest {
  username: string;
  email: string;
  password: string;
  firstName?: string;
  lastName?: string;
  roleId: number;
}

export interface UpdateUserRequest {
  email?: string;
  firstName?: string;
  lastName?: string;
  roleId?: number;
  isActive?: boolean;
  newPassword?: string;
}

export interface UserResponse extends BaseResponse {
  id: number;
  username: string;
  email: string;
  firstName?: string;
  lastName?: string;
  roleId: number;
  roleName: string;
  isActive: boolean;
  createdAt: Date;
  lastLoginAt?: Date;
}

export interface UserListResponse extends BaseResponse {
  users: UserResponse[];
}

export interface RoleResponse {
  id: number;
  name: string;
  description?: string;
}

export interface RoleListResponse extends BaseResponse {
  roles: RoleResponse[];
}

@Injectable({ providedIn: 'root' })
export class UserApiService {
  private baseUrl = `${environment.apiUrl}/UserManagement`;

  constructor(private http: HttpClient) {}

  /**
   * Get all users (Admin only)
   */
  getAllUsers(): Observable<UserListResponse> {
    return this.http.get<UserListResponse>(this.baseUrl);
  }

  /**
   * Get user by ID (Admin only)
   */
  getUserById(id: number): Observable<UserResponse> {
    return this.http.get<UserResponse>(`${this.baseUrl}/${id}`);
  }

  /**
   * Create a new user (Admin only)
   */
  createUser(user: UserRequest): Observable<UserResponse> {
    return this.http.post<UserResponse>(this.baseUrl, user);
  }

  /**
   * Update an existing user (Admin only)
   */
  updateUser(id: number, user: UpdateUserRequest): Observable<UserResponse> {
    return this.http.put<UserResponse>(`${this.baseUrl}/${id}`, user);
  }

  /**
   * Delete a user (Admin only)
   */
  deleteUser(id: number): Observable<BaseResponse> {
    return this.http.delete<BaseResponse>(`${this.baseUrl}/${id}`);
  }

  /**
   * Get all available roles
   */
  getAllRoles(): Observable<RoleListResponse> {
    return this.http.get<RoleListResponse>(`${this.baseUrl}/roles`);
  }
}
