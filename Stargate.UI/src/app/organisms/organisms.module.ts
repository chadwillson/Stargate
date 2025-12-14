import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MoleculesModule } from '../molecules/molecules.module';
import { AtomsModule } from '../atoms/atoms.module';
import { AuthFormComponent } from './auth-form/auth-form.component';
import { DataTableSectionComponent } from './data-table-section/data-table-section.component';
import { PersonTableComponent } from './person-table/person-table.component';
import { AstronautDutyTableComponent } from './astronaut-duty-table/astronaut-duty-table.component';
import { UserManagementTableComponent } from './user-management-table/user-management-table.component';
import { UserFormModalComponent } from './user-form-modal/user-form-modal.component';
import { ForgotPasswordFormComponent } from './forgot-password-form/forgot-password-form.component';
import { ResetPasswordFormComponent } from './reset-password-form/reset-password-form.component';
import { RegisterFormComponent } from './register-form/register-form.component';
import { PersonnelSearchComponent } from './personnel-search/personnel-search.component';
import { AstronautDutyFormComponent } from './astronaut-duty-form/astronaut-duty-form.component';
import { DutyHistoryDetailComponent } from './duty-history-detail/duty-history-detail.component';

@NgModule({
  declarations: [AuthFormComponent, DataTableSectionComponent, PersonTableComponent, AstronautDutyTableComponent, UserManagementTableComponent, UserFormModalComponent, ForgotPasswordFormComponent, ResetPasswordFormComponent, RegisterFormComponent, PersonnelSearchComponent, AstronautDutyFormComponent, DutyHistoryDetailComponent],
  imports: [CommonModule, FormsModule, MoleculesModule, AtomsModule],
  exports: [AuthFormComponent, DataTableSectionComponent, PersonTableComponent, AstronautDutyTableComponent, UserManagementTableComponent, UserFormModalComponent, ForgotPasswordFormComponent, ResetPasswordFormComponent, RegisterFormComponent, PersonnelSearchComponent, AstronautDutyFormComponent, DutyHistoryDetailComponent]
})
export class OrganismsModule {}
