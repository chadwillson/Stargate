import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TemplatesModule } from '../templates/templates.module';
import { OrganismsModule } from '../organisms/organisms.module';
import { AtomsModule } from '../atoms/atoms.module';
import { DashboardPageComponent } from './dashboard-page/dashboard-page.component';
import { AstronautDutyPageComponent } from './astronaut-duty-page/astronaut-duty-page.component';
import { LoginPageComponent } from './login-page/login-page.component';

@NgModule({
  declarations: [DashboardPageComponent, AstronautDutyPageComponent, LoginPageComponent],
  imports: [CommonModule, FormsModule, TemplatesModule, OrganismsModule, AtomsModule],
  exports: [DashboardPageComponent, AstronautDutyPageComponent, LoginPageComponent]
})
export class PagesModule {}
