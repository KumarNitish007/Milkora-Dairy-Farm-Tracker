import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Sale, CreateSaleRequest, UpdatePaymentStatusRequest } from '../models';

@Injectable({ providedIn: 'root' })
export class SaleService {
  private api = inject(ApiService);

  getByRange(start: string, end: string, paymentStatus?: string, buyer?: string): Observable<Sale[]> {
    return this.api.get<Sale[]>('/sales', { start, end, paymentStatus, buyer });
  }
  create(req: CreateSaleRequest): Observable<{ saleId: string }> {
    return this.api.post<{ saleId: string }>('/sales', req);
  }
  updatePayment(id: string, req: UpdatePaymentStatusRequest): Observable<void> {
    return this.api.put<void>(`/sales/${id}/payment`, req);
  }
}
