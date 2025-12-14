import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-button',
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.scss'],
  standalone: false
})
export class ButtonComponent {
  @Input() kind: 'primary' | 'secondary' | 'ghost' = 'primary';
  @Input() type: 'button' | 'submit' = 'button';
  @Input() disabled = false;
  @Output() pressed = new EventEmitter<void>();

  onClick(): void {
    // Only emit pressed event for type="button", not for submit buttons
    if (!this.disabled && this.type === 'button') {
      this.pressed.emit();
    }
  }
}
