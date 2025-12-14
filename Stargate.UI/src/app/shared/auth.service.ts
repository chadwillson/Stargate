import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginRequest, LoginResponse, ForgotPasswordRequest, ResetPasswordRequest, BaseResponse } from './models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = `${environment.apiUrl}/Auth`;
  private tokenKey = 'stargate_auth_token';
  private usernameKey = 'stargate_username';
  private roleKey = 'stargate_role';

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(private http: HttpClient) {}

  /**
   * POST - Login with username and password
   * POST /api/Auth/login
   */
  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, credentials).pipe(
      tap(response => {
        if (response.success && response.token) {
          this.setToken(response.token);
          this.setUsername(response.username || credentials.username);
          this.setRole(this.extractRoleFromToken(response.token));
          this.isAuthenticatedSubject.next(true);
        }
      })
    );
  }

  /**
   * POST - Logout
   * POST /api/Auth/logout
   */
  logout(): Observable<any> {
    return this.http.post(`${this.baseUrl}/logout`, {}).pipe(
      tap(() => {
        this.clearAuth();
      })
    );
  }

  /**
   * Clear authentication data and update state
   */
  clearAuth(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.usernameKey);
    localStorage.removeItem(this.roleKey);
    this.isAuthenticatedSubject.next(false);
  }

  /**
   * Get the stored auth token
   */
  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  /**
   * Get the stored username
   */
  getUsername(): string | null {
    return localStorage.getItem(this.usernameKey);
  }

  /**
   * Check if user has a token
   */
  hasToken(): boolean {
    return !!this.getToken();
  }

  /**
   * Check if user is authenticated
   */
  isAuthenticated(): boolean {
    return this.hasToken();
  }

  private setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  private setUsername(username: string): void {
    localStorage.setItem(this.usernameKey, username);
  }

  /**
   * Get the stored role
   */
  getRole(): string | null {
    return localStorage.getItem(this.roleKey);
  }

  /**
   * Check if user has a specific role
   */
  hasRole(role: string): boolean {
    return this.getRole() === role;
  }

  /**
   * Check if user is admin
   */
  isAdmin(): boolean {
    return this.hasRole('Admin');
  }

  private setRole(role: string): void {
    localStorage.setItem(this.roleKey, role);
  }

  /**
   * POST - Forgot password - send reset link to email
   * POST /api/Auth/forgot-password
   */
  forgotPassword(email: string): Observable<BaseResponse> {
    const request: ForgotPasswordRequest = { email };
    return this.http.post<BaseResponse>(`${this.baseUrl}/forgot-password`, request);
  }

  /**
   * POST - Reset password with token
   * POST /api/Auth/reset-password
   */
  resetPassword(token: string, newPassword: string): Observable<BaseResponse> {
    const request: ResetPasswordRequest = { token, newPassword };
    return this.http.post<BaseResponse>(`${this.baseUrl}/reset-password`, request);
  }

  /**
   * POST - Register new user account
   * POST /api/Auth/register
   */
  register(request: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/register`, request);
  }

  /**
   * Extract role from JWT token
   */
  private extractRoleFromToken(token: string): string {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      // Try short form first, then fall back to full URI form
      return payload['role'] || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 'User';
    } catch {
      return 'User';
    }
  }
}
