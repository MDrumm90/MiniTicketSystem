import { Routes } from '@angular/router';
import { MainLayout } from './layout/main-layout/main-layout';

export const routes: Routes = [
  {
    path: '',
    component: MainLayout,
    children: [
      {
        path: 'tickets',
        children: [
          {
            path: '',
            loadComponent: () =>
              import('./features/tickets/pages/ticket-list/ticket-list')
                .then(m => m.TicketList)
          },
          {
            path: 'new',
            loadComponent: () =>
              import('./features/tickets/pages/ticket-form/ticket-form')
                .then(m => m.TicketForm)
          },
          {
            path: ':id/edit',
            loadComponent: () =>
              import('./features/tickets/pages/ticket-form/ticket-form')
                .then(m => m.TicketForm)
          }
        ]
      }
    ]
  },
  {
    path: '',
    redirectTo: 'tickets',
    pathMatch: 'full'
  }
];