import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const token = authService.getToken();
  
  if (req.url.includes('/auth/login')) {
    return next(req);
  }

  // Set withCredentials to send cookies with cross-origin requests
  const clonedReq = token 
    ? req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        },
        withCredentials: true
      })
    : req.clone({
        withCredentials: true
      });
  
  return next(clonedReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // Handle 401 errors - logout if token was present but request failed
      const isBookmarksRequest = clonedReq.url.includes('/bookmarks');
      const isLoginPage = router.url.includes('/login');
      
      if (error.status === 401 && token && !isLoginPage && !isBookmarksRequest) {
        authService.logout();
        router.navigate(['/login']);
      }
      return throwError(() => error);
    })
  );
};

