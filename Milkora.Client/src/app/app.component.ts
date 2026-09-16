import { Component, inject } from '@angular/core';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { PwaUpdateService } from './core/services/pwa-update.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [MainLayoutComponent],
  template: `<app-main-layout />`,
})
export class AppComponent {
  constructor() {
    inject(PwaUpdateService).init();
  }
}
