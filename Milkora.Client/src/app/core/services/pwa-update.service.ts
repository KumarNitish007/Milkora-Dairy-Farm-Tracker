import { inject, Injectable } from '@angular/core';
import { SwUpdate, VersionReadyEvent } from '@angular/service-worker';
import { filter } from 'rxjs';
import { NotificationService } from './notification.service';

/**
 * Watches the service worker for a freshly deployed version and prompts a reload.
 * No-op when the service worker isn't enabled (dev, or unsupported browsers).
 */
@Injectable({ providedIn: 'root' })
export class PwaUpdateService {
  private swUpdate = inject(SwUpdate);
  private notify = inject(NotificationService);

  init(): void {
    if (!this.swUpdate.isEnabled) return;

    this.swUpdate.versionUpdates
      .pipe(filter((e): e is VersionReadyEvent => e.type === 'VERSION_READY'))
      .subscribe(() => {
        this.notify.success('A new version is available — reloading…');
        setTimeout(() => document.location.reload(), 1500);
      });
  }
}
