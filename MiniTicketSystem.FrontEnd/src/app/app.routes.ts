import { Routes } from '@angular/router';
import { MainLayout } from './layout/main-layout/main-layout';

export const routes: Routes = [
  {
    path: '',
    component: MainLayout,
    children: [
      {
        path: 'tickets',
        loadComponent: () =>
          import('./features/tickets/pages/ticket-list/ticket-list')
            .then(m => m.TicketList)
      }
    ]
  },
  {
    path: '',
    redirectTo: 'tickets',
    pathMatch: 'full'
  }
];