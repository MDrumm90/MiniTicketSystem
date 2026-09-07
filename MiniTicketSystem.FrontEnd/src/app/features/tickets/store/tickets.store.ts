import { inject } from '@angular/core';
import { CreateTicket } from '../models/create-ticket.model';
import { finalize, tap } from 'rxjs/operators';
import {
    signalStore,
    withState,
    withMethods,
    patchState
} from '@ngrx/signals';

import { Ticket, TicketStatus } from '../models/ticket.model';
import { TicketService } from '../data-access/ticket.service';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

type TicketViewMode = 'all' | 'paged';

type TicketsState = {
    viewMode: TicketViewMode;

    tickets: Ticket[];
    saving: boolean;
    loading: boolean;
    error: string | null;

    searchTerm: string;
    statusFilter: TicketStatus | undefined;

    page: number;
    pageSize: number;

    totalCount: number;
    totalPages: number;
};


export const TicketsStore = signalStore(
    withState<TicketsState>({
        viewMode: 'all',

        tickets: [],
        saving: false,
        loading: false,
        error: null,

        searchTerm: '',
        statusFilter: undefined,

        page: 1,
        pageSize: 10,

        totalCount: 0,
        totalPages: 0
    }),

    withMethods((store) => {
        const ticketService = inject(TicketService);
        const searchSubject = new Subject<string>();

        searchSubject
            .pipe(
                debounceTime(400),
                distinctUntilChanged()
            )
            .subscribe(searchTerm => {
                patchState(store, {
                    searchTerm,
                    page: 1
                });

                if (store.viewMode() === 'paged') {
                    loadPaged();
                }
            });

        const loadAll = () => {
            patchState(store, {
                loading: true,
                error: null
            });

            ticketService.getAll().subscribe({
                next: tickets => {
                    patchState(store, {
                        tickets,
                        loading: false
                    });
                },

                error: () => {
                    patchState(store, {
                        loading: false,
                        error: 'Failed to load tickets'
                    });
                }
            });
        };

        const loadPaged = () => {
            patchState(store, {
                loading: true,
                error: null
            });

            ticketService.getPaged(
                store.statusFilter(),
                store.searchTerm(),
                store.page(),
                store.pageSize()
            ).subscribe({
                next: result => {
                    patchState(store, {
                        tickets: result.items,
                        totalCount: result.totalCount,
                        totalPages: result.totalPages,
                        loading: false
                    });
                },

                error: () => {
                    patchState(store, {
                        loading: false,
                        error: 'Failed to load tickets'
                    });
                }
            });
        };

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


        return {
            createTicket,
            loadAll,
            loadPaged,
            setSearchTerm(searchTerm: string) {
                searchSubject.next(searchTerm);
            },

            setStatusFilter(status: TicketStatus | undefined) {
                patchState(store, {
                    statusFilter: status,
                    page: 1
                });

                if (store.viewMode() === 'paged') {
                    loadPaged();
                }
            },

            setPageSize(pageSize: number) {
                patchState(store, {
                    pageSize,
                    page: 1
                });

                if (store.viewMode() === 'paged') {
                    loadPaged();
                }
            },
            setPage(page: number) {
                if (page < 1 || page > store.totalPages()) {
                    return;
                }

                patchState(store, {
                    page
                });

                if (store.viewMode() === 'paged') {
                    loadPaged();
                }
            },
            setViewMode(mode: TicketViewMode) {
                patchState(store, {
                    viewMode: mode,
                    page: 1
                });

                if (mode === 'all') {
                    loadAll();
                } else {
                    loadPaged();
                }
            }
        };
    })
);