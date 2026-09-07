import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TicketFormStore } from '../../store/ticket-form.store';
import { TicketStatus } from '../../models/ticket.model';

// display labels for the numeric TicketStatus enum
const STATUS_LABELS: Record<TicketStatus, string> = {
  [TicketStatus.Open]: 'Open',
  [TicketStatus.InProgress]: 'In Progress',
  [TicketStatus.Closed]: 'Closed'
};

@Component({
  selector: 'app-ticket-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './ticket-form.html',
  styleUrl: './ticket-form.scss',
  providers: [TicketFormStore]
})



export class TicketForm implements OnInit {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);
  readonly store = inject(TicketFormStore);

  // null id means create mode; non-null means editing that ticket
  private ticketId: string | null = null;
  isEditMode = false;

  readonly statusOptions = Object.values(TicketStatus)
    .filter((value): value is number => typeof value === 'number')
    .map((value) => ({ value: value as TicketStatus, label: STATUS_LABELS[value as TicketStatus] }));

  readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required]],
    description: [''],
    status: [TicketStatus.Open]
  });

  ngOnInit(): void {
    this.ticketId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = this.ticketId !== null;

    if (this.ticketId) {
      this.store.loadTicket(this.ticketId).subscribe({
        next: (ticket) => {
          this.form.patchValue({
            title: ticket.title,
            description: ticket.description,
            status: ticket.status
          });
        },

        error: () => {
          // error jest już zapisany w Store
        }
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.store.saving() || this.store.loading()) {
      return;
    }

    if (this.isEditMode && this.ticketId) {
      this.store.updateTicket({
        id: this.ticketId,
        ...this.form.getRawValue()
      }).subscribe({
        next: () => {
          this.router.navigate(['/tickets']);
        },

        error: () => {
          // error jest już zapisany w Store
        }
      });
      return;
    }

    const { title, description } = this.form.getRawValue();

    this.store.createTicket({ title, description }).subscribe({
      next: () => {
        this.router.navigate(['/tickets']);
      },

      error: () => {
        // error jest już zapisany w Store
      }
    });
  }
}