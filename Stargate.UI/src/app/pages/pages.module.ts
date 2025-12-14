import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TemplatesModule } from '../templates/templates.module';
import { OrganismsModule } from '../organisms/organisms.module';
import { AtomsModule } from '../atoms/atoms.module';
import { DashboardPageComponent } from './dashboard-page/dashboard-page.component';
import { AstronautDutyPageComponent } from './astronaut-duty-page/astronaut-duty-page.component';
import { LoginPageComponent } from './login-page/login-page.component';
import { AdminPageComponent } from './admin-page/admin-page.component';
import { ForgotPasswordPageComponent } from './forgot-password-page/forgot-password-page.component';
import { ResetPasswordPageComponent } from './reset-password-page/reset-password-page.component';
import { RegisterPageComponent } from './register-page/register-page.component';

@NgModule({
  declarations: [DashboardPageComponent, AstronautDutyPageComponent, LoginPageComponent, AdminPageComponent, ForgotPasswordPageComponent, ResetPasswordPageComponent, RegisterPageComponent],
  imports: [CommonModule, FormsModule, RouterModule, TemplatesModule, OrganismsModule, AtomsModule],
  exports: [DashboardPageComponent, AstronautDutyPageComponent, LoginPageComponent, AdminPageComponent]
})
export class PagesModule {}
