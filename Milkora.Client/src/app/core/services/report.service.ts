import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { DashboardSummary, MonthlyReport, ProfitLossReport, PerAnimalYield } from '../models';

@Injectable({ providedIn: 'root' })
export class ReportService {
  private api = inject(ApiService);

  dashboard(date?: string): Observable<DashboardSummary> {
    return this.api.get<DashboardSummary>('/reports/dashboard', { date });
  }
  monthly(year: number, month: number): Observable<MonthlyReport> {
    return this.api.get<MonthlyReport>('/reports/monthly', { year, month });
  }
  profitLoss(start: string, end: string): Observable<ProfitLossReport> {
    return this.api.get<ProfitLossReport>('/reports/profitloss', { start, end });
  }
  perAnimalYield(start: string, end: string): Observable<PerAnimalYield[]> {
    return this.api.get<PerAnimalYield[]>('/reports/peranimalyield', { start, end });
  }
}
