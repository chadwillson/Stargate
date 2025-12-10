import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardPageComponent } from './pages/dashboard-page/dashboard-page.component';
import { AstronautDutyPageComponent } from './pages/astronaut-duty-page/astronaut-duty-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { AuthGuard } from './shared/auth.guard';

const routes: Routes = [
  { path: '', redirectTo: '/people', pathMatch: 'full' },
  { path: 'login', component: LoginPageComponent },
  { path: 'people', component: DashboardPageComponent, canActivate: [AuthGuard] },
  { path: 'astronaut-duties', component: AstronautDutyPageComponent, canActivate: [AuthGuard] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
