import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { DefaultTemplateComponent } from './default-template/default-template.component';

@NgModule({
  declarations: [DefaultTemplateComponent],
  imports: [CommonModule],
  exports: [DefaultTemplateComponent]
})
export class TemplatesModule {}
