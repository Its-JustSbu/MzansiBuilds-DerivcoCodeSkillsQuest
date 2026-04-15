import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Auth } from './auth';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(Auth);
  const router = inject(Router);

  if (authService.token() && !authService.isTokenExpired()) {
    return true; // Allow navigation
  } else {
    return router.parseUrl('/login'); // Redirect to login
  }
};
