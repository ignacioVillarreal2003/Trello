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
    loadComponent: () => import('./articles/BoardDashboard/board-dashboard/board-dashboard.component').then(m => m.BoardDashboardComponent),
  },
  {
    path: 'board-view/:id',
    loadComponent: () => import('./articles/board-view/board-view.component').then(m => m.BoardViewComponent),
  },
  {
    path: 'card-view/:id',
    loadComponent: () => import('./articles/CardView/card-view/card-view.component').then(m => m.CardViewComponent),
  },
  {
    path: 'user-menu',
    loadComponent: () => import('./articles/user-menu/user-menu.component').then(m => m.UserMenuComponent),
  }
];
