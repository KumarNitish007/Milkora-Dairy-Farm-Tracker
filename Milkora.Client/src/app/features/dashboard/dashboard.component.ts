import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ReportService } from '../../core/services/report.service';
import { HealthService } from '../../core/services/health.service';
import { FeedService } from '../../core/services/feed.service';
import { MlService } from '../../core/services/ml.service';
import { DashboardSummary, DueReminder, FeedItem, MilkForecast, MilkAnomaly } from '../../core/models';
import { PageHeaderComponent } from '../../shared/components/page-header.component';
import { StatCardComponent } from '../../shared/components/stat-card.component';
import { SpinnerComponent } from '../../shared/components/spinner.component';
import { today } from '../../shared/utils/dates';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, PageHeaderComponent, StatCardComponent, SpinnerComponent],
  templateUrl: './dashboard.component.html',
})
export class DashboardComponent implements OnInit {
  private reports = inject(ReportService);
  private health = inject(HealthService);
  private feed = inject(FeedService);
  private ml = inject(MlService);

  loading = signal(true);
  summary = signal<DashboardSummary | null>(null);
  reminders = signal<DueReminder[]>([]);
  lowStock = signal<FeedItem[]>([]);
  forecast = signal<MilkForecast | null>(null);
  anomalies = signal<MilkAnomaly[]>([]);
  todayStr = today();

  // Sparkline scaling for the forecast bars.
  maxForecast = computed(() => Math.max(...(this.forecast()?.points.map(p => p.upperBound) ?? [1]), 1));

  ngOnInit() {
    forkJoin({
      summary: this.reports.dashboard(this.todayStr),
      reminders: this.health.getReminders(7),
      lowStock: this.feed.getLowStock(),
      forecast: this.ml.getForecast(7),
      anomalies: this.ml.getAnomalies(30),
    }).subscribe({
      next: r => {
        this.summary.set(r.summary);
        this.reminders.set(r.reminders);
        this.lowStock.set(r.lowStock);
        this.forecast.set(r.forecast);
        // One alert per animal (server sorts worst-first).
        const seen = new Set<string>();
        this.anomalies.set(r.anomalies.anomalies.filter(a => {
          const k = a.animalId ?? a.date;
          if (seen.has(k)) return false;
          seen.add(k);
          return true;
        }));
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  barHeight(value: number): number {
    return Math.max(6, Math.round((value / this.maxForecast()) * 100));
  }
}
