import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-auth-form',
  templateUrl: './auth-form.component.html',
  styleUrls: ['./auth-form.component.scss'],
  standalone: false
})
export class AuthFormComponent {
  @Input() title = 'Sign in';
  @Input() error: string | null = null;
  @Output() submitAuth = new EventEmitter<{ email: string; password: string }>();

  model = { email: '', password: '' };

  onSubmit(): void {
    this.submitAuth.emit({ ...this.model });
  }
}
