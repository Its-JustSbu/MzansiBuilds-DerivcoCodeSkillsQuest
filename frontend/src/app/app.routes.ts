import { Routes } from '@angular/router';
import { Login } from './auth/login/login';
import { Register } from './auth/register/register';
import { Notfound } from './notfound/notfound';
import { NavigationComponent } from './main/navigation/navigation.component';
import { HomeComponent } from './main/home/home.component';
import { MyProjectsComponent } from './main/my-projects/my-projects.component';
import { CelebrationWallComponent } from './main/celebration-wall/celebration-wall.component';
import { ProfileComponent } from './main/profile/profile.component';
import { authGuard } from './utils/services/auth-guard';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'not-found', component: Notfound },
  {
    path: 'portal',
    component: NavigationComponent,
    children: [
      { path: 'home', component: HomeComponent },
      { path: 'projects', component: MyProjectsComponent },
      { path: 'celebrations', component: CelebrationWallComponent },
      { path: 'profile', component: ProfileComponent },
      { path: 'portal', redirectTo: '/portal/home', pathMatch: 'full' },
    ],
    canActivate: [authGuard]
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'not-found', pathMatch: 'full' },
];
