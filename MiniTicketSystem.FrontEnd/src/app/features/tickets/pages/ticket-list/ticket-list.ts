import { Component, inject } from '@angular/core';

import { TicketsStore } from '../../store/tickets.store';
import { TicketStatus } from '../../models/ticket.model';

@Component({
  selector: 'app-ticket-list',
  standalone: true,
  imports: [],
  templateUrl: './ticket-list.html',
  styleUrl: './ticket-list.scss',
  providers: [TicketsStore]
})
export class TicketList {
  readonly store = inject(TicketsStore);

  readonly TicketStatus = TicketStatus;

  readonly statuses = [
    TicketStatus.Open,
    TicketStatus.InProgress,
    TicketStatus.Closed
  ];

  constructor() {
    this.store.loadAll();
  }

  onSearchChange(event: Event): void {
  const value = (event.target as HTMLInputElement).value;

  this.store.setSearchTerm(value);
}

  getStatusLabel(status: TicketStatus): string {
    switch (status) {
      case TicketStatus.Open:
        return 'Open';

      case TicketStatus.InProgress:
        return 'In progress';

      case TicketStatus.Closed:
        return 'Closed';

      default:
        return 'Unknown';
    }
  }

  onStatusChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;

    const status =
      value === ''
        ? undefined
        : Number(value) as TicketStatus;

    this.store.setStatusFilter(status);
  }
}