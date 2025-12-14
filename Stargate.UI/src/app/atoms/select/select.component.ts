import { Component, EventEmitter, Input, Output } from '@angular/core';

export interface SelectOption {
  value: any;
  label: string;
}

@Component({
  selector: 'app-select',
  standalone: false,
  templateUrl: './select.component.html',
  styleUrl: './select.component.scss'
})
export class SelectComponent {
  @Input() id = '';
  @Input() name = '';
  @Input() options: SelectOption[] = [];
  @Input() value: any = '';
  @Input() disabled = false;
  @Input() required = false;
  @Output() valueChange = new EventEmitter<any>();

  onChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    // Parse as number if the value is numeric, otherwise keep as string
    const newValue = !isNaN(Number(target.value)) && target.value !== ''
      ? Number(target.value)
      : target.value;
    this.valueChange.emit(newValue);
  }
}
