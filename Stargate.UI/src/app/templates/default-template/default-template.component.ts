import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-default-template',
  templateUrl: './default-template.component.html',
  styleUrls: ['./default-template.component.scss'],
  standalone: false
})
export class DefaultTemplateComponent {
  @Input() pageTitle = '';
}
