import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { HealthService } from '../../core/services/health.service';
import { FeedService } from '../../core/services/feed.service';
import { SaleService } from '../../core/services/sale.service';
import { DueReminder, FeedItem, Sale } from '../../core/models';
import { PageHeaderComponent } from '../../shared/components/page-header.component';
import { SpinnerComponent } from '../../shared/components/spinner.component';
import { daysAgo, today } from '../../shared/utils/dates';

@Component({
  selector: 'app-reminders',
  standalone: true,
  imports: [CommonModule, RouterLink, PageHeaderComponent, SpinnerComponent],
  templateUrl: './reminders.component.html',
})
export class RemindersComponent implements OnInit {
  private health = inject(HealthService);
  private feed = inject(FeedService);
  private sales = inject(SaleService);

  loading = signal(true);
  reminders = signal<DueReminder[]>([]);
  lowStock = signal<FeedItem[]>([]);
  pending = signal<Sale[]>([]);

  ngOnInit() {
    forkJoin({
      reminders: this.health.getReminders(30),
      lowStock: this.feed.getLowStock(),
      pending: this.sales.getByRange(daysAgo(365), today(), 'Pending'),
    }).subscribe({
      next: r => { this.reminders.set(r.reminders); this.lowStock.set(r.lowStock); this.pending.set(r.pending); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }
}
