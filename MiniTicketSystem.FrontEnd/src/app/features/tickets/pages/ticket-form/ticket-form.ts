import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TicketsStore } from '../../store/tickets.store';


@Component({
  selector: 'app-ticket-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './ticket-form.html',
  styleUrl: './ticket-form.scss',
  providers: [TicketsStore]
})



export class TicketForm {
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  readonly store = inject(TicketsStore);

  readonly form = this.fb.nonNullable.group({
    title: ['', [Validators.required]],
    description: ['']
  });

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.store.createTicket(this.form.getRawValue()).subscribe({
      next: () => {
        this.router.navigate(['/tickets']);
      },

      error: () => {
        // error jest już zapisany w Store
      }
    });
  }
}