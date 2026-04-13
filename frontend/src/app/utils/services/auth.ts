import { inject, Injectable, signal } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Storage } from './storage';
import { Router } from '@angular/router';
import { Api } from './api';
import { Messagebox } from './messagebox';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private apiService = inject(Api);
  private snackService = inject(Messagebox);
  private storageService = inject(Storage);
  private jwtHelper = new JwtHelperService();
  private router = inject(Router);
  token = signal<string>('');

  constructor() {
    const tempToken = this.storageService.getItem('bearer-token') as string;

    if (tempToken) {
      this.token.update(() => tempToken);
    } else {
      this.router.navigate(['/login']);
    }
  }

  public isTokenExpired(): boolean {
    return this.jwtHelper.isTokenExpired(this.token());
  }

  public refreshToken() {
    const rt = this.storageService.getItem('refreshToken');

    if (!rt) {
      this.storageService.removeItem('token');
      this.storageService.removeItem('refreshToken');
      this.snackService.openWarning('Unauthorized!');
      this.token.update(() => '');
      this.router.navigate(['/login']);
      return;
    }

    this.apiService.get(`User/RefreshToken/${encodeURI(rt)}`).subscribe({
      next: (res: any) => {
        this.storageService.setItem('token', res.token);
        this.storageService.setItem('refreshToken', res.refreshToken);
        this.token.update(() => this.storageService.getItem('token') as string);
      },
      error: (error: any) => {
        this.storageService.removeItem('token');
        this.storageService.removeItem('refreshToken');
        this.snackService.openError(error.error.message);
        this.token.update(() => '');
        this.router.navigate(['/login']);
      },
    });
  }
}
