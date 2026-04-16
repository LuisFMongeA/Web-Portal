import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { LoginPage } from './features/auth/pages/login-page/login-page';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    component: LoginPage
  },
  {
    path: 'home',
    loadComponent: () =>
      import('./features/todo/pages/todo-page/todo-page').then((m) => m.TodoPage),
    canActivate: [authGuard]
  },
  {
    path: 'about',
    loadComponent: () =>
      import('./features/about/pages/about-page/about-page').then((m) => m.AboutPage),
    canActivate: [authGuard]
  },

  { path: '**', redirectTo: 'login' }
];
