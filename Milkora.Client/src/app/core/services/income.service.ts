import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Income, CreateIncomeRequest } from '../models';

@Injectable({ providedIn: 'root' })
export class IncomeService {
  private api = inject(ApiService);

  getByRange(start: string, end: string, category?: string): Observable<Income[]> {
    return this.api.get<Income[]>('/income', { start, end, category });
  }
  create(req: CreateIncomeRequest): Observable<{ incomeId: string }> {
    return this.api.post<{ incomeId: string }>('/income', req);
  }
}
