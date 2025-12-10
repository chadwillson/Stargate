import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ButtonComponent } from './button/button.component';
import { LabelComponent } from './label/label.component';
import { TextInputComponent } from './text-input/text-input.component';

@NgModule({
  declarations: [ButtonComponent, LabelComponent, TextInputComponent],
  imports: [CommonModule],
  exports: [ButtonComponent, LabelComponent, TextInputComponent]
})
export class AtomsModule {}
