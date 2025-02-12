import { HttpInterceptorFn } from '@angular/common/http';
import {inject} from "@angular/core";
import {SessionServiceService} from '../../services/session-service.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const sessionServiceService: SessionServiceService = inject(SessionServiceService);
  const authorization: string = `Bearer ${sessionServiceService.getToken()}`;

  const modifiedRequest = req.clone({
    setHeaders: {
      Authorization: authorization
    }
  });
  return next(modifiedRequest);
};
