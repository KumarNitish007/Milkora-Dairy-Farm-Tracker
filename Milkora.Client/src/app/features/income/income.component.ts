import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IncomeService } from '../../core/services/income.service';
import { NotificationService } from '../../core/services/notification.service';
import { Income, CreateIncomeRequest } from '../../core/models';
import { INCOME_CATEGORIES } from '../../core/constants/options';
import { PageHeaderComponent } from '../../shared/components/page-header.component';
import { SpinnerComponent } from '../../shared/components/spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { monthStart, today } from '../../shared/utils/dates';

@Component({
  selector: 'app-income',
  standalone: true,
  imports: [CommonModule, FormsModule, PageHeaderComponent, SpinnerComponent, EmptyStateComponent],
  templateUrl: './income.component.html',
})
export class IncomeComponent implements OnInit {
  private svc = inject(IncomeService);
  private notify = inject(NotificationService);

  categories = INCOME_CATEGORIES;
  start = monthStart();
  end = today();
  categoryFilter = '';

  items = signal<Income[]>([]);
  loading = signal(true);
  saving = signal(false);
  showForm = signal(false);
  form: Partial<CreateIncomeRequest> = {};

  total = computed(() => this.items().reduce((s, x) => s + x.amount, 0));

  ngOnInit() { this.load(); }

  load() {
    this.loading.set(true);
    this.svc.getByRange(this.start, this.end, this.categoryFilter || undefined).subscribe({
      next: x => { this.items.set(x); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  openCreate() { this.form = { incomeDate: today(), category: 'Milk Sale', amount: 0 }; this.showForm.set(true); }
  close() { this.showForm.set(false); }

  save() {
    this.saving.set(true);
    this.svc.create({
      incomeDate: this.form.incomeDate || today(),
      category: this.form.category || 'Other',
      description: this.form.description ?? null,
      amount: Number(this.form.amount) || 0,
      notes: this.form.notes ?? null,
    }).subscribe({
      next: () => { this.notify.success('Income added.'); this.saving.set(false); this.close(); this.load(); },
      error: () => this.saving.set(false),
    });
  }
}
