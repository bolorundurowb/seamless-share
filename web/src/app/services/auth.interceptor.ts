import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from './auth.service';
import { inject } from '@angular/core';
import { appVersion } from '../app.version';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const updatedHeaderRequest = req.clone({
    setHeaders: {
      'X-App-Version': `Web;${appVersion}`,
    },
  })

  if (authService.isAuthenticated()) {
    const token = authService.getToken();

    if (token) {
      const authReq = updatedHeaderRequest.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      });

      return next(authReq);
    }
  }

  return next(updatedHeaderRequest);
};
