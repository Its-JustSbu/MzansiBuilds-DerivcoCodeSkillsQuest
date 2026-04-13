import { HttpInterceptorFn } from '@angular/common/http';
import { Auth } from './auth';
import { inject } from '@angular/core';
import { Loader } from './loader';
import { finalize } from 'rxjs';

export const interceptorInterceptor: HttpInterceptorFn = (req, next) => {
  const loaderService = inject(Loader);
  const auth = inject(Auth);
  loaderService.show();

  // if (auth.isTokenExpired()) {
  //   auth.refreshToken();
  // }

  if (auth.token()) {
    // Clone the request to add the authentication header.
    const newReq = req.clone({
      headers: req.headers.append('Authentication', `Bearer ${auth.token()}`),
    });

    return next(newReq).pipe(finalize(() => loaderService.hide()));
  }

  return next(req).pipe(finalize(() => loaderService.hide()));
};
