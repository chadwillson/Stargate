import { ErrorHandler, Injectable, Injector } from '@angular/core';
import { UILoggingService } from './ui-logging.service';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(private injector: Injector) {}

  handleError(error: Error | any): void {
    // Get the logging service (use injector to avoid circular dependency)
    const loggingService = this.injector.get(UILoggingService);

    // Log to console for development
    console.error('Global error caught:', error);

    // Prepare error message
    let errorMessage = 'An unexpected error occurred';

    if (error instanceof Error) {
      errorMessage = error.message;
    } else if (typeof error === 'string') {
      errorMessage = error;
    } else if (error?.message) {
      errorMessage = error.message;
    }

    // Send to backend logging
    loggingService.error(errorMessage, error);
  }
}
