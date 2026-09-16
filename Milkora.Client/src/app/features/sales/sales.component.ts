import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SaleService } from '../../core/services/sale.service';
import { NotificationService } from '../../core/services/notification.service';
import { Sale, CreateSaleRequest } from '../../core/models';
import { PAYMENT_STATUSES, PAYMENT_MODES } from '../../core/constants/options';
import { PageHeaderComponent } from '../../shared/components/page-header.component';
import { SpinnerComponent } from '../../shared/components/spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { monthStart, today } from '../../shared/utils/dates';

@Component({
  selector: 'app-sales',
  standalone: true,
  imports: [CommonModule, FormsModule, PageHeaderComponent, SpinnerComponent, EmptyStateComponent],
  templateUrl: './sales.component.html',
})
export class SalesComponent implements OnInit {
  private svc = inject(SaleService);
  private notify = inject(NotificationService);

  statuses = PAYMENT_STATUSES;
  modes = PAYMENT_MODES;

  start = monthStart();
  end = today();
  statusFilter = '';

  sales = signal<Sale[]>([]);
  loading = signal(true);
  saving = signal(false);
  showForm = signal(false);
  form: Partial<CreateSaleRequest> = {};

  total = computed(() => this.sales().reduce((s, x) => s + x.totalAmount, 0));

  ngOnInit() { this.load(); }

  load() {
    this.loading.set(true);
    this.svc.getByRange(this.start, this.end, this.statusFilter || undefined).subscribe({
      next: s => { this.sales.set(s); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  openCreate() {
    this.form = { saleDate: today(), paymentStatus: 'Pending', paymentMode: 'Cash', quantityLitres: 0, ratePerLitre: 0 };
    this.showForm.set(true);
  }
  close() { this.showForm.set(false); }

  save() {
    if (!this.form.buyerName) { this.notify.warn('Buyer name is required.'); return; }
    this.saving.set(true);
    this.svc.create({
      saleDate: this.form.saleDate || today(),
      buyerName: this.form.buyerName!,
      quantityLitres: Number(this.form.quantityLitres) || 0,
      ratePerLitre: Number(this.form.ratePerLitre) || 0,
      paymentStatus: this.form.paymentStatus || 'Pending',
      paymentMode: this.form.paymentMode ?? null,
      notes: this.form.notes ?? null,
    }).subscribe({
      next: () => { this.notify.success('Sale recorded.'); this.saving.set(false); this.close(); this.load(); },
      error: () => this.saving.set(false),
    });
  }

  markPaid(s: Sale) {
    this.svc.updatePayment(s.saleId, { paymentStatus: 'Paid', paymentMode: s.paymentMode || 'Cash' }).subscribe({
      next: () => { this.notify.success('Marked as paid.'); this.load(); },
    });
  }
}
