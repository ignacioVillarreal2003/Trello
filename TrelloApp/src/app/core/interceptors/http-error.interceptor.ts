import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AlertService } from '../services/alert.service';
import { catchError, throwError } from 'rxjs';
import {Router} from '@angular/router';

export const httpErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const alertService: AlertService = inject(AlertService);
  const router: Router = inject(Router);
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      switch (error.status) {
        case 400:
          alertService.ErrorMessage('Bad Request: The server could not understand the request.');
          break;
        case 401:
          router.navigate(['/']);
          break;
        case 403:
          router.navigate(['/error']);
          break;
        case 404:
          alertService.ErrorMessage('Not Found: The requested resource could not be found.');
          break;
        case 500:
          alertService.ErrorMessage('Internal Server Error: Something went wrong on the server.');
          break;
        case 502:
          alertService.ErrorMessage('Bad Gateway: The server received an invalid response from the upstream server.');
          break;
        case 503:
          alertService.ErrorMessage('Service Unavailable: The server is currently unable to handle the request.');
          break;
        case 504:
          alertService.ErrorMessage('Gateway Timeout: The server did not receive a timely response from the upstream server.');
          break;
        default:
          alertService.ErrorMessage(error.message || 'An unknown error occurred.');
          break;
      }
      return throwError(() => error);
    })
  );
};
