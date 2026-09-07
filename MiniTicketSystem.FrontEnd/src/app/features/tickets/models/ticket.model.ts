export enum TicketStatus {
  Open = 0,
  InProgress = 1,
  Closed = 2
}

export interface Ticket {
  id: string;
  title: string;
  description: string;
  status: TicketStatus;
  updatedAt: string | null;
  dateCreated: string;
  dateClosed: string | null;
}