import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Ticket, TicketStatus } from '../models/ticket.model';
import { PagedResult } from '../models/paged-result.model';
import { CreateTicket } from '../models/create-ticket.model';
import { UpdateTicket } from '../models/update-ticket.model';
@Injectable({
  providedIn: 'root'
})
export class TicketService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'http://localhost:5019/Tickets';

 create(ticket: CreateTicket): Observable<Ticket> {
    return this.http.post<Ticket>(this.apiUrl, ticket);
  }

  update(ticket: UpdateTicket): Observable<Ticket> {
  return this.http.put<Ticket>(
    `${this.apiUrl}/${ticket.id}`,
    ticket
  );
}

  getAll(): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(this.apiUrl);
  }

  getById(id: string): Observable<Ticket> {
    return this.http.get<Ticket>(`${this.apiUrl}/${id}`);
  }

  getPaged(
    status?: TicketStatus,
    search?: string,
    page = 1,
    pageSize = 10
  ): Observable<PagedResult<Ticket>> {

    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    if (status !== undefined) {
      params = params.set('status', status);
    }

    if (search?.trim()) {
      params = params.set('search', search.trim());
    }

    return this.http.get<PagedResult<Ticket>>(
      `${this.apiUrl}/paged`,
      { params }
    );
  }
}