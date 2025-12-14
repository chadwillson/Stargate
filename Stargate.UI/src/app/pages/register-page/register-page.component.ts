import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../shared/auth.service';
import { RegisterFormData } from '../../organisms/register-form/register-form.component';

@Component({
  selector: 'app-register-page',
  standalone: false,
  templateUrl: './register-page.component.html',
  styleUrl: './register-page.component.scss'
})
export class RegisterPageComponent {
  error: string | null = null;
  loading = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(formData: RegisterFormData): void {
    this.error = null;
    this.loading = true;

    // Register via public auth endpoint
    this.authService.register({
      username: formData.username,
      email: formData.email,
      password: formData.password,
      firstName: formData.firstName,
      lastName: formData.lastName
    }).subscribe({
      next: (response) => {
        if (response.success) {
          // Redirect to login page with success message
          this.router.navigate(['/login'], {
            queryParams: { registered: 'true' }
          });
        } else {
          this.error = response.message || 'Failed to create account';
          this.loading = false;
        }
      },
      error: (err) => {
        console.error('Registration error:', err);
        this.error = err.error?.message || 'Failed to create account. Please try again.';
        this.loading = false;
      }
    });
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}
