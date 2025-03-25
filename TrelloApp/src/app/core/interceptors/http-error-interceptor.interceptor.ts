import {HttpErrorResponse, HttpInterceptorFn} from '@angular/common/http';
import {inject} from '@angular/core';
import {AlertService} from '../services/alert.service';
import {catchError, throwError} from 'rxjs';

export const httpErrorInterceptorInterceptor: HttpInterceptorFn = (req, next) => {
  const alertService: AlertService = inject(AlertService);
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      alertService.ErrorMessage(error.message || 'An error occurred');
      return throwError(() => error)
    })
  );
};
