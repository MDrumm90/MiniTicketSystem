import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Ticket } from '../models/ticket.model';

@Injectable({
  providedIn: 'root'
})
export class TicketService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'http://localhost:5019/Tickets';

  getAll(): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(this.apiUrl);
  }
}