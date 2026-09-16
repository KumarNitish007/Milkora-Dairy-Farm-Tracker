import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  template: `
    <div class="empty">
      <div style="font-size:2.2rem">{{ icon }}</div>
      <p>{{ message }}</p>
      <ng-content></ng-content>
    </div>
  `,
})
export class EmptyStateComponent {
  @Input() message = 'Nothing here yet.';
  @Input() icon = '🐄';
}
