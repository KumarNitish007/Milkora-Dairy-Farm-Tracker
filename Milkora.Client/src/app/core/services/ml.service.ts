import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { MilkForecast, AnomalyReport, MilkHealth } from '../models';

@Injectable({ providedIn: 'root' })
export class MlService {
  private api = inject(ApiService);

  getForecast(days = 7): Observable<MilkForecast> {
    return this.api.get<MilkForecast>('/ml/forecast', { days });
  }
  getAnomalies(lookbackDays = 30): Observable<AnomalyReport> {
    return this.api.get<AnomalyReport>('/ml/anomalies', { lookbackDays });
  }
  getHealthChecks(lookbackDays = 45): Observable<MilkHealth[]> {
    return this.api.get<MilkHealth[]>('/ml/health-check', { lookbackDays });
  }
}
