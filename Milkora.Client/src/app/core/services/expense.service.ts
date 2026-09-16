import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Expense, CreateExpenseRequest, UpdateExpenseRequest } from '../models';

@Injectable({ providedIn: 'root' })
export class ExpenseService {
  private api = inject(ApiService);

  getByRange(start: string, end: string, category?: string): Observable<Expense[]> {
    return this.api.get<Expense[]>('/expenses', { start, end, category });
  }
  create(req: CreateExpenseRequest): Observable<{ expenseId: string }> {
    return this.api.post<{ expenseId: string }>('/expenses', req);
  }
  update(id: string, req: UpdateExpenseRequest): Observable<void> {
    return this.api.put<void>(`/expenses/${id}`, req);
  }
  delete(id: string): Observable<void> {
    return this.api.delete<void>(`/expenses/${id}`);
  }
}
