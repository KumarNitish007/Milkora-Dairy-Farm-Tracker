import { Routes } from '@angular/router';

// Lazy-loaded standalone feature components (one per module).
export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  { path: 'dashboard', title: 'Dashboard · Milkora', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
  { path: 'animals', title: 'Animals · Milkora', loadComponent: () => import('./features/animals/animals.component').then(m => m.AnimalsComponent) },
  { path: 'milk-log', title: 'Milk Log · Milkora', loadComponent: () => import('./features/milk-log/milk-log.component').then(m => m.MilkLogComponent) },
  { path: 'sales', title: 'Sales · Milkora', loadComponent: () => import('./features/sales/sales.component').then(m => m.SalesComponent) },
  { path: 'expenses', title: 'Expenses · Milkora', loadComponent: () => import('./features/expenses/expenses.component').then(m => m.ExpensesComponent) },
  { path: 'income', title: 'Income · Milkora', loadComponent: () => import('./features/income/income.component').then(m => m.IncomeComponent) },
  { path: 'health', title: 'Health · Milkora', loadComponent: () => import('./features/health/health.component').then(m => m.HealthComponent) },
  { path: 'breeding', title: 'Breeding · Milkora', loadComponent: () => import('./features/breeding/breeding.component').then(m => m.BreedingComponent) },
  { path: 'feed', title: 'Feed · Milkora', loadComponent: () => import('./features/feed/feed.component').then(m => m.FeedComponent) },
  { path: 'reports', title: 'Reports · Milkora', loadComponent: () => import('./features/reports/reports.component').then(m => m.ReportsComponent) },
  { path: 'reminders', title: 'Reminders · Milkora', loadComponent: () => import('./features/reminders/reminders.component').then(m => m.RemindersComponent) },
  { path: 'settings', title: 'Settings · Milkora', loadComponent: () => import('./features/settings/settings.component').then(m => m.SettingsComponent) },
  { path: '**', redirectTo: 'dashboard' },
];
