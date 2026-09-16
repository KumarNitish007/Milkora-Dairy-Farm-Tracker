import { Injectable, signal } from '@angular/core';

export type ToastKind = 'success' | 'error' | 'warn';
export interface Toast { id: number; kind: ToastKind; message: string; }

@Injectable({ providedIn: 'root' })
export class NotificationService {
  readonly toasts = signal<Toast[]>([]);
  private seq = 0;

  success(message: string) { this.push('success', message); }
  error(message: string) { this.push('error', message); }
  warn(message: string) { this.push('warn', message); }

  dismiss(id: number) {
    this.toasts.update(list => list.filter(t => t.id !== id));
  }

  private push(kind: ToastKind, message: string) {
    const id = ++this.seq;
    this.toasts.update(list => [...list, { id, kind, message }]);
    setTimeout(() => this.dismiss(id), kind === 'error' ? 6000 : 3500);
  }
}
