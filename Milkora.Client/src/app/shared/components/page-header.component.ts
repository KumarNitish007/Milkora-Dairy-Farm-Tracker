import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-page-header',
  standalone: true,
  template: `
    <div class="page-head">
      <div>
        <h1 class="page-title">{{ title }}</h1>
        @if (subtitle) { <p class="page-sub">{{ subtitle }}</p> }
      </div>
      <div class="flex items-center gap"><ng-content></ng-content></div>
    </div>
  `,
})
export class PageHeaderComponent {
  @Input() title = '';
  @Input() subtitle?: string;
}
