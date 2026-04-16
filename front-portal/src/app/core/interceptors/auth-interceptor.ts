import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthResponse } from '../../features/auth/models/auth.model';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const accessToken = authService.accessToken();
  
  const modifiedReq = accessToken ? req.clone({
  setHeaders: {
    Authorization: `Bearer ${accessToken}`
  }
}) : req;

  return next(modifiedReq).pipe(
    catchError((error) => {
      if (error.status === 401) {

        return authService.refresh().pipe(
          switchMap((response: AuthResponse) => {
            const retryReq = req.clone({
              setHeaders: {
                Authorization: `Bearer ${response.accessToken}`
              }
            });
            return next(retryReq);
          }),

          catchError(() => {
            authService.logout();
            router.navigate(['/login']);
            return throwError(() => error);
          })
        );
      }
  return throwError(() => error);
})




  );
};
