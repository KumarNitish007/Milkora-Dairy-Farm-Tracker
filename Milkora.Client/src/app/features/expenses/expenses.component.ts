import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExpenseService } from '../../core/services/expense.service';
import { NotificationService } from '../../core/services/notification.service';
import { Expense, CreateExpenseRequest } from '../../core/models';
import { EXPENSE_CATEGORIES, PAYMENT_MODES } from '../../core/constants/options';
import { PageHeaderComponent } from '../../shared/components/page-header.component';
import { SpinnerComponent } from '../../shared/components/spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { monthStart, today } from '../../shared/utils/dates';

@Component({
  selector: 'app-expenses',
  standalone: true,
  imports: [CommonModule, FormsModule, PageHeaderComponent, SpinnerComponent, EmptyStateComponent],
  templateUrl: './expenses.component.html',
})
export class ExpensesComponent implements OnInit {
  private svc = inject(ExpenseService);
  private notify = inject(NotificationService);

  categories = EXPENSE_CATEGORIES;
  modes = PAYMENT_MODES;

  start = monthStart();
  end = today();
  categoryFilter = '';

  items = signal<Expense[]>([]);
  loading = signal(true);
  saving = signal(false);
  showForm = signal(false);
  form: Partial<CreateExpenseRequest> = {};

  total = computed(() => this.items().reduce((s, x) => s + x.amount, 0));

  ngOnInit() { this.load(); }

  load() {
    this.loading.set(true);
    this.svc.getByRange(this.start, this.end, this.categoryFilter || undefined).subscribe({
      next: x => { this.items.set(x); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  openCreate() { this.form = { expenseDate: today(), category: 'Feed', paymentMode: 'Cash', amount: 0 }; this.showForm.set(true); }
  close() { this.showForm.set(false); }

  save() {
    this.saving.set(true);
    this.svc.create({
      expenseDate: this.form.expenseDate || today(),
      category: this.form.category || 'Miscellaneous',
      description: this.form.description ?? null,
      amount: Number(this.form.amount) || 0,
      paymentMode: this.form.paymentMode ?? null,
      notes: this.form.notes ?? null,
    }).subscribe({
      next: () => { this.notify.success('Expense added.'); this.saving.set(false); this.close(); this.load(); },
      error: () => this.saving.set(false),
    });
  }

  remove(e: Expense) {
    if (!confirm('Delete this expense?')) return;
    this.svc.delete(e.expenseId).subscribe({ next: () => { this.notify.success('Expense deleted.'); this.load(); } });
  }
}
