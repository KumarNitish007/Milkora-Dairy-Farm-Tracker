import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { ReportService } from '../../core/services/report.service';
import { ProfitLossReport, PerAnimalYield, CategoryAmount } from '../../core/models';
import { PageHeaderComponent } from '../../shared/components/page-header.component';
import { SpinnerComponent } from '../../shared/components/spinner.component';
import { monthStart, today } from '../../shared/utils/dates';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, FormsModule, PageHeaderComponent, SpinnerComponent],
  templateUrl: './reports.component.html',
})
export class ReportsComponent implements OnInit {
  private reports = inject(ReportService);

  start = monthStart();
  end = today();

  loading = signal(true);
  pl = signal<ProfitLossReport | null>(null);
  yield = signal<PerAnimalYield[]>([]);

  ngOnInit() { this.load(); }

  load() {
    this.loading.set(true);
    forkJoin({
      pl: this.reports.profitLoss(this.start, this.end),
      yield: this.reports.perAnimalYield(this.start, this.end),
    }).subscribe({
      next: r => { this.pl.set(r.pl); this.yield.set(r.yield); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  /** Bar width % relative to the largest amount in the set. */
  barWidth(item: CategoryAmount, set: CategoryAmount[]): number {
    const max = Math.max(...set.map(x => x.amount), 1);
    return Math.round((item.amount / max) * 100);
  }

  maxYield(): number {
    return Math.max(...this.yield().map(y => y.totalMilkLitres), 1);
  }
}
