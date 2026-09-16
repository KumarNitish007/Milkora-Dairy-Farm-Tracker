import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { SKIP_ERROR_TOAST } from '../interceptors/error.interceptor';

export type SyncState = 'synced' | 'pending' | 'syncing' | 'error' | 'offline';

/**
 * Drives the sync indicator in the navbar.
 *
 * In the DESKTOP build the app is served by the local host, which exposes
 * /api/desktop/sync — this service talks to the real MilkoraSyncManager there.
 * In the WEB build those routes 404 (probed silently), and the chip simply
 * reflects online/offline.
 */
@Injectable({ providedIn: 'root' })
export class SyncStatusService {
  private http = inject(HttpClient);
  private readonly base = environment.apiBaseUrl;
  private readonly silent = new HttpContext().set(SKIP_ERROR_TOAST, true);

  readonly state = signal<SyncState>(navigator.onLine ? 'synced' : 'offline');
  readonly lastSyncedAt = signal<Date | null>(navigator.onLine ? new Date() : null);
  readonly desktopMode = signal(false);

  constructor() {
    window.addEventListener('online', () => { this.state.set('synced'); this.refreshFromDesktop(); });
    window.addEventListener('offline', () => this.state.set('offline'));
    this.refreshFromDesktop();
  }

  /** Manual "Sync Now": triggers the real engine on desktop, no-op-ish on web. */
  syncNow(): void {
    if (!navigator.onLine) { this.state.set('offline'); return; }
    this.state.set('syncing');
    this.http.post<any>(`${this.base}/desktop/sync/now`, {}, { context: this.silent }).subscribe({
      next: r => this.applyDesktop(r?.data),
      error: () => { this.state.set('synced'); this.lastSyncedAt.set(new Date()); },
    });
  }

  private refreshFromDesktop(): void {
    this.http.get<any>(`${this.base}/desktop/sync/status`, { context: this.silent }).subscribe({
      next: r => this.applyDesktop(r?.data),
      error: () => { /* web build: no desktop endpoint */ },
    });
  }

  private applyDesktop(d: any): void {
    if (!d) return;
    this.desktopMode.set(true);
    this.state.set(mapState(d.state));
    if (d.lastSyncedAt) this.lastSyncedAt.set(new Date(d.lastSyncedAt));
  }

  label(): string {
    switch (this.state()) {
      case 'synced': return 'Synced';
      case 'pending': return 'Pending';
      case 'syncing': return 'Syncing…';
      case 'error': return 'Sync error';
      case 'offline': return 'Offline';
    }
  }
}

// Maps the C# SyncState enum (numeric) to the UI state.
function mapState(numeric: number): SyncState {
  switch (numeric) {
    case 1: return 'syncing';
    case 2: return 'synced';
    case 3: return 'pending';
    case 4: return 'offline';
    case 5: return 'error';
    default: return 'pending'; // Idle
  }
}
