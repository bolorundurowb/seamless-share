import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./home/home.page').then((m) => m.HomePage),
  },
  {
    path: 'shares/:shareCode',
    loadComponent: () => import('./share/share.page').then((m) => m.SharePage),
  }
];
