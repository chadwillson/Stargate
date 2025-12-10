import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { AtomsModule } from '../atoms/atoms.module';
import { FormFieldComponent } from './form-field/form-field.component';

@NgModule({
  declarations: [FormFieldComponent],
  imports: [CommonModule, AtomsModule],
  exports: [FormFieldComponent]
})
export class MoleculesModule {}
