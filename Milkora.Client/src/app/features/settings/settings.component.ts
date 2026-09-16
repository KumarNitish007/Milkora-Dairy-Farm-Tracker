import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SyncStatusService } from '../../core/services/sync-status.service';
import { NotificationService } from '../../core/services/notification.service';
import { PageHeaderComponent } from '../../shared/components/page-header.component';

interface FarmSettings {
  farmName: string;
  ownerName: string;
  defaultRate: number;
  currency: string;
}

const KEY = 'milkora.settings';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule, PageHeaderComponent],
  templateUrl: './settings.component.html',
})
export class SettingsComponent implements OnInit {
  sync = inject(SyncStatusService);
  private notify = inject(NotificationService);

  settings: FarmSettings = { farmName: 'Milkora Dairy Farm', ownerName: '', defaultRate: 55, currency: '₹' };
  lastSynced = signal<Date | null>(null);

  ngOnInit() {
    try {
      const raw = localStorage.getItem(KEY);
      if (raw) this.settings = { ...this.settings, ...JSON.parse(raw) };
    } catch { /* ignore */ }
    this.lastSynced.set(this.sync.lastSyncedAt());
  }

  save() {
    try {
      localStorage.setItem(KEY, JSON.stringify(this.settings));
      this.notify.success('Settings saved on this device.');
    } catch {
      this.notify.error('Could not save settings.');
    }
  }

  syncNow() {
    this.sync.syncNow();
    setTimeout(() => this.lastSynced.set(this.sync.lastSyncedAt()), 900);
  }

  clearLocal() {
    if (!confirm('Clear locally saved settings on this device? Farm data on the server is not affected.')) return;
    try { localStorage.removeItem(KEY); } catch { /* ignore */ }
    this.settings = { farmName: 'Milkora Dairy Farm', ownerName: '', defaultRate: 55, currency: '₹' };
    this.notify.warn('Local settings cleared.');
  }
}
