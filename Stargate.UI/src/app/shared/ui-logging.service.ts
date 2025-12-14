import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, of } from 'rxjs';
import { environment } from '../../environments/environment';
import { BaseResponse } from './models';

export interface UILogRequest {
  level: 'Info' | 'Warning' | 'Error';
  message: string;
  url?: string;
  userAgent?: string;
  stackTrace?: string;
  additionalData?: string;
}

@Injectable({ providedIn: 'root' })
export class UILoggingService {
  private baseUrl = `${environment.apiUrl}/UILogging`;

  constructor(private http: HttpClient) {}

  /**
   * Send a log message to the backend
   */
  log(level: 'Info' | 'Warning' | 'Error', message: string, additionalData?: any): Observable<BaseResponse> {
    const logRequest: UILogRequest = {
      level,
      message,
      url: window.location.href,
      userAgent: navigator.userAgent,
      additionalData: additionalData ? JSON.stringify(additionalData) : undefined
    };

    return this.http.post<BaseResponse>(this.baseUrl, logRequest).pipe(
      catchError(error => {
        // If logging fails, just log to console and don't throw
        console.error('Failed to send log to server:', error);
        return of({ success: false, message: 'Failed to send log' } as BaseResponse);
      })
    );
  }

  /**
   * Log an info message
   */
  info(message: string, additionalData?: any): void {
    console.log(`[INFO] ${message}`, additionalData);
    this.log('Info', message, additionalData).subscribe();
  }

  /**
   * Log a warning message
   */
  warning(message: string, additionalData?: any): void {
    console.warn(`[WARNING] ${message}`, additionalData);
    this.log('Warning', message, additionalData).subscribe();
  }

  /**
   * Log an error message
   */
  error(message: string, error?: Error | any, additionalData?: any): void {
    console.error(`[ERROR] ${message}`, error, additionalData);

    const logRequest: UILogRequest = {
      level: 'Error',
      message,
      url: window.location.href,
      userAgent: navigator.userAgent,
      stackTrace: error?.stack || error?.toString(),
      additionalData: additionalData ? JSON.stringify(additionalData) : undefined
    };

    this.http.post<BaseResponse>(this.baseUrl, logRequest).pipe(
      catchError(err => {
        console.error('Failed to send error log to server:', err);
        return of({ success: false, message: 'Failed to send error log' } as BaseResponse);
      })
    ).subscribe();
  }
}
