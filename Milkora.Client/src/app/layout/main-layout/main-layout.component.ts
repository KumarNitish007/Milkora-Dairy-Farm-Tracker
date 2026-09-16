import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from '../navbar/navbar.component';
import { ToastHostComponent } from '../../shared/components/toast-host.component';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, ToastHostComponent],
  template: `
    <app-navbar />
    <main class="container">
      <router-outlet />
    </main>
    <app-toast-host />
  `,
})
export class MainLayoutComponent {}
