import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardPageComponent } from './pages/dashboard-page/dashboard-page.component';
import { AstronautDutyPageComponent } from './pages/astronaut-duty-page/astronaut-duty-page.component';

const routes: Routes = [
  { path: '', redirectTo: '/people', pathMatch: 'full' },
  { path: 'people', component: DashboardPageComponent },
  { path: 'astronaut-duties', component: AstronautDutyPageComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
