import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { MilkLog, CreateMilkLogRequest, UpdateMilkLogRequest } from '../models';

@Injectable({ providedIn: 'root' })
export class MilkLogService {
  private api = inject(ApiService);

  getByDate(date: string): Observable<MilkLog[]> {
    return this.api.get<MilkLog[]>('/milklog', { date });
  }
  getByRange(start: string, end: string, animalId?: string): Observable<MilkLog[]> {
    return this.api.get<MilkLog[]>('/milklog', { start, end, animalId });
  }
  create(req: CreateMilkLogRequest): Observable<{ logId: string }> {
    return this.api.post<{ logId: string }>('/milklog', req);
  }
  update(id: string, req: UpdateMilkLogRequest): Observable<void> {
    return this.api.put<void>(`/milklog/${id}`, req);
  }
}
