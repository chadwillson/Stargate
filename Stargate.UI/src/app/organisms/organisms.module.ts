import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MoleculesModule } from '../molecules/molecules.module';
import { AtomsModule } from '../atoms/atoms.module';
import { AuthFormComponent } from './auth-form/auth-form.component';
import { DataTableSectionComponent } from './data-table-section/data-table-section.component';
import { PersonTableComponent } from './person-table/person-table.component';
import { AstronautDutyTableComponent } from './astronaut-duty-table/astronaut-duty-table.component';

@NgModule({
  declarations: [AuthFormComponent, DataTableSectionComponent, PersonTableComponent, AstronautDutyTableComponent],
  imports: [CommonModule, FormsModule, MoleculesModule, AtomsModule],
  exports: [AuthFormComponent, DataTableSectionComponent, PersonTableComponent, AstronautDutyTableComponent]
})
export class OrganismsModule {}
