import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-label',
  template: `<label [for]="for" class="form-label">{{ text }}<span *ngIf="required" class="required-asterisk">*</span></label>`,
  styleUrls: ['./label.component.scss'],
  standalone: false
})
export class LabelComponent {
  @Input() for = '';
  @Input() text = '';
  @Input() required = false;
}
