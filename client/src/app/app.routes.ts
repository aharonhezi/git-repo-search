import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/search',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadComponent: () => import('./features/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'search',
    loadComponent: () => import('./features/search/search.component').then(m => m.SearchComponent),
    canActivate: [authGuard]
  },
  {
    path: 'bookmarks',
    loadComponent: () => import('./features/bookmarks/bookmarks.component').then(m => m.BookmarksComponent),
    canActivate: [authGuard]
  },
  {
    path: '**',
    redirectTo: '/search'
  }
];

