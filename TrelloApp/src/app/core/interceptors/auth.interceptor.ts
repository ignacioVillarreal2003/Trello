import { HttpInterceptorFn } from '@angular/common/http';
import {inject} from "@angular/core";
import {SessionService} from '../services/session/session.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const sessionService: SessionService = inject(SessionService);
  const authorization: string = `Bearer ${sessionService.getSessionData()?.accessToken}`;

  const modifiedRequest = req.clone({
    setHeaders: {
      Authorization: authorization
    }
  });
  return next(modifiedRequest);
};
