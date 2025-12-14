import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-reset-password-form',
  standalone: false,
  templateUrl: './reset-password-form.component.html',
  styleUrl: './reset-password-form.component.scss'
})
export class ResetPasswordFormComponent {
  @Input() error: string | null = null;
  @Input() success: string | null = null;
  @Input() loading = false;
  @Output() submitPassword = new EventEmitter<{ newPassword: string; confirmPassword: string }>();
  @Output() cancel = new EventEmitter<void>();

  model = { newPassword: '', confirmPassword: '' };

  onSubmit(): void {
    this.submitPassword.emit({ ...this.model });
  }

  onCancel(): void {
    this.cancel.emit();
  }
}
