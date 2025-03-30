import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import {provideHttpClient, withInterceptors} from "@angular/common/http";
import {authInterceptor} from "./core/interceptors/auth.interceptor";
import {httpErrorInterceptor} from './core/interceptors/http-error.interceptor';
import {CookieService} from 'ngx-cookie-service';

export const appConfig: ApplicationConfig = {
  providers: [
    CookieService,
    provideHttpClient(
      withInterceptors([authInterceptor, httpErrorInterceptor])
    ),
    provideRouter(routes),
    provideZoneChangeDetection({ eventCoalescing: true })
  ]
};
