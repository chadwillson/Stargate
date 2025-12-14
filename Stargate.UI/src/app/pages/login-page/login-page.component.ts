import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../shared/auth.service';
import { LoginRequest } from '../../shared/models';

@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss'],
  standalone: false
})
export class LoginPageComponent {
  error: string | null = null;
  success: string | null = null;
  loading = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    // Check if user just registered
    this.route.queryParams.subscribe(params => {
      if (params['registered'] === 'true') {
        this.success = 'Account created successfully! Please sign in.';
      }
    });
  }

  onSubmit(credentials: LoginRequest): void {
    this.error = null;
    this.loading = true;

    this.authService.login(credentials).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          // Get the return URL from route parameters or default to '/people'
          const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/people';
          this.router.navigate([returnUrl]);
        } else {
          this.error = response.message || 'Login failed';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'An error occurred during login';
        console.error(err);
      }
    });
  }
}
