import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginRequest, LoginResponse } from './models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = `${environment.apiUrl}/Auth`;
  private tokenKey = 'stargate_auth_token';
  private usernameKey = 'stargate_username';

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
}
