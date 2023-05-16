import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NZ_I18N } from 'ng-zorro-antd/i18n';
import { en_US } from 'ng-zorro-antd/i18n';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SharedModule } from './shared/shared.module';
import { DefaultInterceptor } from './interceptors/default.interceptor';
import { CookieService } from 'ngx-cookie-service';
import { registerLocaleData } from '@angular/common';
import en from '@angular/common/locales/en';
import { AppInitializerProvider } from './services/app-initializer.service';
import { AkitaNgDevtools } from '@datorama/akita-ngdevtools';
import { environment } from '../environments/environment';
registerLocaleData(en);


@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    AppRoutingModule,
    BrowserModule,
    BrowserAnimationsModule,
    SharedModule,
    environment.production ? [] : AkitaNgDevtools.forRoot(),
  ],
  providers: [
    AppInitializerProvider,
    { provide: HTTP_INTERCEPTORS, useClass: DefaultInterceptor, multi: true },
    { provide: NZ_I18N, useValue: en_US },
    [CookieService],
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
