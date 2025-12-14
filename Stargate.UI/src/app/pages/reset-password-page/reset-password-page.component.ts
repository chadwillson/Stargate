import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../shared/auth.service';

@Component({
  selector: 'app-reset-password-page',
  standalone: false,
  templateUrl: './reset-password-page.component.html',
  styleUrl: './reset-password-page.component.scss'
})
export class ResetPasswordPageComponent implements OnInit {
  token: string = '';
  error: string | null = null;
  success: string | null = null;
  loading = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.token = this.route.snapshot.queryParams['token'] || '';

    if (!this.token) {
      this.error = 'Invalid or missing reset token';
    }
  }

  onSubmit(passwords: { newPassword: string; confirmPassword: string }): void {
    this.error = null;
    this.success = null;

    if (!passwords.newPassword || passwords.newPassword.length < 6) {
      this.error = 'Password must be at least 6 characters';
      return;
    }

    if (passwords.newPassword !== passwords.confirmPassword) {
      this.error = 'Passwords do not match';
      return;
    }

    if (!this.token) {
      this.error = 'Invalid or missing reset token';
      return;
    }

    this.loading = true;

    this.authService.resetPassword(this.token, passwords.newPassword).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.success = response.message || 'Password reset successful! You can now login with your new password.';
        } else {
          this.error = response.message || 'Failed to reset password';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'An error occurred while resetting your password';
        console.error(err);
      }
    });
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}
