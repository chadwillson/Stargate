import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-form-field',
  templateUrl: './form-field.component.html',
  styleUrls: ['./form-field.component.scss'],
  standalone: false
})
export class FormFieldComponent {
  @Input() label = '';
  @Input() type: string = 'text';
  @Input() name = '';
  @Input() value = '';
  @Input() placeholder = '';
  @Input() error: string | null = null;
  @Output() valueChange = new EventEmitter<string>();
}
