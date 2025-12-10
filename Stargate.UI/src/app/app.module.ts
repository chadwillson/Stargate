import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, provideHttpClient, withInterceptors } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AtomsModule } from './atoms/atoms.module';
import { MoleculesModule } from './molecules/molecules.module';
import { OrganismsModule } from './organisms/organisms.module';
import { TemplatesModule } from './templates/templates.module';
import { PagesModule } from './pages/pages.module';
import { authInterceptor } from './shared/auth.interceptor';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    AtomsModule,
    MoleculesModule,
    OrganismsModule,
    TemplatesModule,
    PagesModule
  ],
  providers: [
    provideHttpClient(withInterceptors([authInterceptor]))
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
