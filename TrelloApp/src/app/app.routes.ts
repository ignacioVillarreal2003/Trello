import { Routes } from '@angular/router';
import {AuthPageComponent} from './features/auth/auth-page/auth-page.component';
import {LoginFormComponent} from './features/auth/login-form/login-form.component';
import {RegisterFormComponent} from './features/auth/register-form/register-form.component';
import {authGuard} from './core/guards/auth.guard';
import {boardAccessGuard} from './core/guards/board-access.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'auth', pathMatch: 'full' },
  {
    path: 'auth',
    component: AuthPageComponent,
    children: [
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      { path: 'login', component: LoginFormComponent },
      { path: 'register', component: RegisterFormComponent },
    ]
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard-page/dashboard-page.component').then(m => m.DashboardPageComponent),
    canActivate: [authGuard]
  },
  {
    path: 'settings',
    loadComponent: () => import('./features/settings/settings-page/settings-page.component').then(m => m.SettingsPageComponent),
    canActivate: [authGuard]
  },
  {
    path: 'card/:id',
    loadComponent: () => import('./features/card/card-page/card-page.component').then(m => m.CardPageComponent),
    canActivate: [authGuard, boardAccessGuard]
  },
  {
    path: 'board/:id',
    loadComponent: () => import('./features/board/board-page/board-page.component').then(m => m.BoardPageComponent),
    canActivate: [authGuard, boardAccessGuard]
  },
  {
    path: 'error',
    loadComponent: () => import('./features/error/error-page/error-page.component').then(m => m.ErrorPageComponent),
  }
];
