import {CanActivateFn, Router} from '@angular/router';
import {SessionService} from '../services/session/session.service';
import {inject} from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const sessionService: SessionService = inject(SessionService);
  const router: Router = inject(Router);
  if (sessionService.getSessionData() != undefined) {
    return true;
  } else {
    router.navigate(['/'], { queryParams: { returnUrl: state.url } });
    return false;
  }
};
