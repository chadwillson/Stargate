import { Component } from '@angular/core';

@Component({
  selector: 'app-label',
  template: `<label class="form-label"><ng-content></ng-content></label>`,
  styleUrls: ['./label.component.scss'],
  standalone: false
})
export class LabelComponent {}
