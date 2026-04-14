import { Component, inject } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { AsyncPipe } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Storage } from '../../utils/storage';
import { Api } from '../../utils/services/api';
import { Messagebox } from '../../utils/messagebox';

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.scss',
  imports: [
    MatToolbarModule,
    MatButtonModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    AsyncPipe,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
  ],
})
export class NavigationComponent {
  snackService = inject(Messagebox);
  router = inject(Router);
  storageService = inject(Storage);
  apiService = inject(Api);
  private breakpointObserver = inject(BreakpointObserver);

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset).pipe(
    map((result) => result.matches),
    shareReplay(),
  );

  logout() {
    this.apiService.get('User/Logout').subscribe({
      next: (res: any) => {
        this.storageService.removeItem('token');
        this.storageService.removeItem('refreshToken');
        this.snackService.openSuccess(res.message as string);
        this.router.navigate(['/login']);
      },
      error: (error: any) => {
        this.snackService.openError(error.error.message);
      },
    });
  }
}
