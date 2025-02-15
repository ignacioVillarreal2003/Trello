import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'auth',
    pathMatch: 'full',
  },
  {
    path: 'auth',
    loadComponent: () => import('./articles/auth/auth.component').then(m => m.AuthComponent),
  },
  {
    path: 'board-dashboard',
    loadComponent: () => import('./articles/board-dashboard/board-dashboard.component').then(m => m.BoardDashboardComponent),
  },
  {
    path: 'board-view/:id',
    loadComponent: () => import('./articles/board-view/board-view.component').then(m => m.BoardViewComponent),
  }
  ,
  {
    path: 'user-menu',
    loadComponent: () => import('./articles/user-menu-modal/user-menu-modal.component').then(m => m.UserMenuModalComponent),
  }
];
