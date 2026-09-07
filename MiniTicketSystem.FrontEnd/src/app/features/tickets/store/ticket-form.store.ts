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
import { TicketService } from '../data-access/ticket.service';

type TicketFormState = {
    saving: boolean;
    error: string | null;
};

export const TicketFormStore = signalStore(
    withState<TicketFormState>({
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

        return {
            createTicket,
            updateTicket
        };
    })
);
