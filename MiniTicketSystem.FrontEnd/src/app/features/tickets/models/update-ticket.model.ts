import { TicketStatus } from './ticket.model';

export interface UpdateTicket {
  id: string;
  title: string;
  description: string;
  status: TicketStatus;
}