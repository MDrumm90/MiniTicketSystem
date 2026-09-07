import { inject } from '@angular/core';
import { finalize, tap } from 'rxjs/operators';
import {
    signalStore,
    withState,
    withMethods,
    patchState
} from '@ngrx/signals';

import { CreateTicket } from '../models/create-ticket.model';
import { UpdateTicket } from '../models/update-ticket.model';
import { Ticket } from '../models/ticket.model';
import { TicketService } from '../data-access/ticket.service';

type TicketFormState = {
    ticket: Ticket | null;
    loading: boolean;
    saving: boolean;
    error: string | null;
};

export const TicketFormStore = signalStore(
    withState<TicketFormState>({
        ticket: null,
        loading: false,
        saving: false,
        error: null
    }),

    withMethods((store) => {
        const ticketService = inject(TicketService);

        const createTicket = (ticket: CreateTicket) => {
            patchState(store, {
                saving: true,
                error: null
            });

            return ticketService.create(ticket).pipe(
                tap({
                    error: () => {
                        patchState(store, {
                            error: 'Failed to create ticket'
                        });
                    }
                }),
                finalize(() => {
                    patchState(store, {
                        saving: false
                    });
                })
            );
        };

        const updateTicket = (ticket: UpdateTicket) => {
            patchState(store, {
                saving: true,
                error: null
            });

            return ticketService.update(ticket).pipe(
                tap({
                    error: () => {
                        patchState(store, {
                            error: 'Failed to update ticket'
                        });
                    }
                }),
                finalize(() => {
                    patchState(store, {
                        saving: false
                    });
                })
            );
        };

        const loadTicket = (id: string) => {
            patchState(store, {
                loading: true,
                error: null
            });

            return ticketService.getById(id).pipe(
                tap({
                    next: (ticket) => {
                        patchState(store, {
                            ticket
                        });
                    },
                    error: () => {
                        patchState(store, {
                            error: 'Failed to load ticket'
                        });
                    }
                }),
                finalize(() => {
                    patchState(store, {
                        loading: false
                    });
                })
            );
        };

        return {
            createTicket,
            updateTicket,
            loadTicket
        };
    })
);
