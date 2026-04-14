import { HttpInterceptorFn } from '@angular/common/http';
import { Auth } from './auth';
import { inject } from '@angular/core';
import { Loader } from '../loader';
import { finalize } from 'rxjs';
import { Storage } from '../storage';
import { Token } from '@angular/compiler';

export const interceptorInterceptor: HttpInterceptorFn = (req, next) => {
  const loaderService = inject(Loader);
  const storageService = inject(Storage);
  const auth = inject(Auth);
  loaderService.show();

  auth.token.update(() => storageService.getItem('token') as string);

  if (auth.token()) {
    if (auth.isTokenExpired()) {
      auth.refreshToken();
    }
    // Clone the request to add the authentication header.
    const newReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${auth.token()}`,
      },
    });

    return next(newReq).pipe(finalize(() => loaderService.hide()));
  }

  return next(req).pipe(finalize(() => loaderService.hide()));
};
