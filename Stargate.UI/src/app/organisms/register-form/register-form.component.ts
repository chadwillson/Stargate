import { Component, EventEmitter, Input, Output } from '@angular/core';

export interface RegisterFormData {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
}

@Component({
  selector: 'app-register-form',
  standalone: false,
  templateUrl: './register-form.component.html',
  styleUrl: './register-form.component.scss'
})
export class RegisterFormComponent {
  @Input() error: string | null = null;
  @Input() loading = false;
  @Output() submitRegistration = new EventEmitter<RegisterFormData>();
  @Output() cancel = new EventEmitter<void>();

  model: RegisterFormData = {
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: ''
  };

  onSubmit(): void {
    if (this.model.password !== this.model.confirmPassword) {
      return;
    }
    this.submitRegistration.emit({ ...this.model });
  }

  onCancel(): void {
    this.cancel.emit();
  }

  get passwordsMatch(): boolean {
    return this.model.password === this.model.confirmPassword;
  }

  get passwordError(): string | null {
    if (this.model.confirmPassword && !this.passwordsMatch) {
      return 'Passwords do not match';
    }
    return null;
  }
}
