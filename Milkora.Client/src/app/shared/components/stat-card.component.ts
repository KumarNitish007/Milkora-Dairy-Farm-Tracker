import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-stat-card',
  standalone: true,
  template: `
    <div class="card stat" [class.accent]="accent">
      <span class="stat-label">{{ label }}</span>
      <span class="stat-value" [class.text-profit]="tone === 'profit'" [class.text-loss]="tone === 'loss'">{{ value }}</span>
      @if (hint) { <span class="stat-hint">{{ hint }}</span> }
    </div>
  `,
  styles: [`
    .stat { display: flex; flex-direction: column; gap: 4px; }
    .stat.accent { background: linear-gradient(135deg, var(--green-700), var(--green-500)); border: none; color: #fff; }
    .stat.accent .stat-label, .stat.accent .stat-hint { color: rgba(255,255,255,.85); }
    .stat.accent .stat-value { color: #fff; }
    .stat-label { font-size: .8rem; color: var(--muted); font-weight: 600; text-transform: uppercase; letter-spacing: .02em; }
    .stat-value { font-size: 1.7rem; font-weight: 800; line-height: 1.1; }
    .stat-hint { font-size: .8rem; color: var(--muted); }
  `],
})
export class StatCardComponent {
  @Input() label = '';
  @Input() value: string | number | null = '';
  @Input() hint?: string;
  @Input() tone: 'default' | 'profit' | 'loss' = 'default';
  @Input() accent = false;
}
