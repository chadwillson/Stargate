import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-forgot-password-form',
  standalone: false,
  templateUrl: './forgot-password-form.component.html',
  styleUrl: './forgot-password-form.component.scss'
})
export class ForgotPasswordFormComponent {
  @Input() error: string | null = null;
  @Input() success: string | null = null;
  @Input() loading = false;
  @Output() submitEmail = new EventEmitter<string>();
  @Output() cancel = new EventEmitter<void>();

  model = { email: '' };

  onSubmit(): void {
    this.submitEmail.emit(this.model.email);
  }

  onCancel(): void {
    this.cancel.emit();
  }
}
