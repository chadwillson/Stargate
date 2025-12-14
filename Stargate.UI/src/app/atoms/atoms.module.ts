import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ButtonComponent } from './button/button.component';
import { LabelComponent } from './label/label.component';
import { TextInputComponent } from './text-input/text-input.component';
import { SelectComponent } from './select/select.component';
import { CheckboxComponent } from './checkbox/checkbox.component';

@NgModule({
  declarations: [ButtonComponent, LabelComponent, TextInputComponent, SelectComponent, CheckboxComponent],
  imports: [CommonModule],
  exports: [ButtonComponent, LabelComponent, TextInputComponent, SelectComponent, CheckboxComponent]
})
export class AtomsModule {}
