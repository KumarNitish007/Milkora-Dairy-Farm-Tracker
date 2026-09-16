import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FeedService } from '../../core/services/feed.service';
import { NotificationService } from '../../core/services/notification.service';
import { FeedItem, CreateFeedItemRequest } from '../../core/models';
import { FEED_UNITS } from '../../core/constants/options';
import { PageHeaderComponent } from '../../shared/components/page-header.component';
import { SpinnerComponent } from '../../shared/components/spinner.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { today } from '../../shared/utils/dates';

@Component({
  selector: 'app-feed',
  standalone: true,
  imports: [CommonModule, FormsModule, PageHeaderComponent, SpinnerComponent, EmptyStateComponent],
  templateUrl: './feed.component.html',
})
export class FeedComponent implements OnInit {
  private svc = inject(FeedService);
  private notify = inject(NotificationService);

  units = FEED_UNITS;
  search = '';

  items = signal<FeedItem[]>([]);
  loading = signal(true);
  saving = signal(false);

  showForm = signal(false);
  form: Partial<CreateFeedItemRequest> = {};

  // stock modal
  showStock = signal(false);
  stockFor = signal<FeedItem | null>(null);
  stockQty = 0;

  ngOnInit() { this.load(); }

  load() {
    this.loading.set(true);
    this.svc.getAll(this.search || undefined).subscribe({
      next: x => { this.items.set(x); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  openCreate() { this.form = { unit: 'kg', purchaseDate: today(), quantityInStock: 0, minimumLevel: 0 }; this.showForm.set(true); }
  close() { this.showForm.set(false); }

  save() {
    if (!this.form.itemName) { this.notify.warn('Item name is required.'); return; }
    this.saving.set(true);
    this.svc.create({
      itemName: this.form.itemName!,
      quantityInStock: Number(this.form.quantityInStock) || 0,
      unit: this.form.unit ?? null,
      purchaseDate: this.form.purchaseDate ?? null,
      cost: this.form.cost ?? null,
      dailyUsage: this.form.dailyUsage ?? null,
      minimumLevel: Number(this.form.minimumLevel) || 0,
      notes: this.form.notes ?? null,
    }).subscribe({
      next: () => { this.notify.success('Feed item added.'); this.saving.set(false); this.close(); this.load(); },
      error: () => this.saving.set(false),
    });
  }

  openStock(f: FeedItem) { this.stockFor.set(f); this.stockQty = f.quantityInStock; this.showStock.set(true); }
  closeStock() { this.showStock.set(false); }

  saveStock() {
    const f = this.stockFor();
    if (!f) return;
    this.saving.set(true);
    this.svc.updateStock(f.inventoryId, { quantityInStock: Number(this.stockQty) || 0 }).subscribe({
      next: () => { this.notify.success('Stock updated.'); this.saving.set(false); this.closeStock(); this.load(); },
      error: () => this.saving.set(false),
    });
  }
}
