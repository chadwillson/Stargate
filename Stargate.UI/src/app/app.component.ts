import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { ThemeService, Theme } from './shared/theme.service';
import { AuthService } from './shared/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  standalone: false
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'Stargate Management System';
  currentTheme: Theme = 'dark';
  isAuthenticated = false;
  username: string | null = null;
  private themeSubscription?: Subscription;
  private authSubscription?: Subscription;

  constructor(
    private themeService: ThemeService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.themeSubscription = this.themeService.theme$.subscribe(theme => {
      this.currentTheme = theme;
    });

    this.authSubscription = this.authService.isAuthenticated$.subscribe(isAuth => {
      this.isAuthenticated = isAuth;
      this.username = isAuth ? this.authService.getUsername() : null;
    });
  }

  ngOnDestroy(): void {
    this.themeSubscription?.unsubscribe();
    this.authSubscription?.unsubscribe();
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },
      error: () => {
        // Even if the API call fails, clear local auth
        this.authService.clearAuth();
        this.router.navigate(['/login']);
      }
    });
  }

  get isDarkMode(): boolean {
    return this.currentTheme === 'dark';
  }

  get isAdmin(): boolean {
    return this.authService.isAdmin();
  }
}
