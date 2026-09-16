import { Component, inject } from '@angular/core';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-toast-host',
  standalone: true,
  template: `
    <div class="toast-host">
      @for (t of notify.toasts(); track t.id) {
        <div class="toast" [class.error]="t.kind === 'error'" [class.warn]="t.kind === 'warn'"
             (click)="notify.dismiss(t.id)">
          {{ t.message }}
        </div>
      }
    </div>
  `,
})
export class ToastHostComponent {
  notify = inject(NotificationService);
}
