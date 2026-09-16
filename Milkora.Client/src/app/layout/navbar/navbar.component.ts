import { Component, ElementRef, HostListener, inject, signal } from '@angular/core';
import { NavigationEnd, Router, RouterLink, RouterLinkActive } from '@angular/router';
import { filter } from 'rxjs';
import { SyncStatusService } from '../../core/services/sync-status.service';

interface NavChild { path: string; label: string; icon: string; }
interface NavGroup { key: string; label: string; icon: string; children: NavChild[]; }

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
})
export class NavbarComponent {
  private router = inject(Router);
  private host = inject(ElementRef<HTMLElement>);
  sync = inject(SyncStatusService);

  menuOpen = signal(false);                    // mobile drawer
  openGroup = signal<string | null>(null);     // desktop dropdown
  currentUrl = signal(this.router.url);

  readonly dashboard: NavChild = { path: '/dashboard', label: 'Dashboard', icon: '🏠' };
  readonly settings: NavChild = { path: '/settings', label: 'Settings', icon: '⚙️' };

  // 12 modules grouped into a few top-level menus → no horizontal scroll.
  readonly groups: NavGroup[] = [
    {
      key: 'herd', label: 'Herd', icon: '🐄', children: [
        { path: '/animals', label: 'Animals', icon: '🐄' },
        { path: '/milk-log', label: 'Milk Log', icon: '🥛' },
        { path: '/health', label: 'Health', icon: '💉' },
        { path: '/breeding', label: 'Breeding', icon: '🐮' },
        { path: '/feed', label: 'Feed Inventory', icon: '🌾' },
      ],
    },
    {
      key: 'finance', label: 'Finance', icon: '💰', children: [
        { path: '/sales', label: 'Sales', icon: '💰' },
        { path: '/expenses', label: 'Expenses', icon: '🧾' },
        { path: '/income', label: 'Income', icon: '📈' },
      ],
    },
    {
      key: 'insights', label: 'Insights', icon: '📊', children: [
        { path: '/reports', label: 'Reports', icon: '📊' },
        { path: '/reminders', label: 'Reminders', icon: '🔔' },
      ],
    },
  ];

  constructor() {
    // Close menus and track the active route on every navigation.
    this.router.events.pipe(filter(e => e instanceof NavigationEnd)).subscribe(e => {
      this.currentUrl.set((e as NavigationEnd).urlAfterRedirects);
      this.closeAll();
    });
  }

  toggleGroup(key: string) { this.openGroup.update(k => (k === key ? null : key)); }
  isGroupActive(g: NavGroup): boolean { return g.children.some(c => this.currentUrl().startsWith(c.path)); }

  toggleMobile() { this.menuOpen.update(v => !v); }
  closeAll() { this.menuOpen.set(false); this.openGroup.set(null); }

  @HostListener('document:click', ['$event'])
  onDocClick(e: MouseEvent) {
    if (!this.host.nativeElement.contains(e.target as Node)) this.openGroup.set(null);
  }

  @HostListener('document:keydown.escape')
  onEscape() { this.closeAll(); }

  @HostListener('window:resize')
  onResize() { if (window.innerWidth > 900) this.menuOpen.set(false); }
}
