import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../shared/auth.service';

@Component({
  selector: 'app-forgot-password-page',
  standalone: false,
  templateUrl: './forgot-password-page.component.html',
  styleUrl: './forgot-password-page.component.scss'
})
export class ForgotPasswordPageComponent {
  error: string | null = null;
  success: string | null = null;
  loading = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(email: string): void {
    this.error = null;
    this.success = null;

    if (!email || !email.includes('@')) {
      this.error = 'Please enter a valid email address';
      return;
    }

    this.loading = true;

    this.authService.forgotPassword(email).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.success = response.message || 'Password reset link sent to your email.';
        } else {
          this.error = response.message || 'Failed to send reset link';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'An error occurred while processing your request';
        console.error(err);
      }
    });
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}
