import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { HealthRecord, DueReminder, CreateHealthRecordRequest } from '../models';

@Injectable({ providedIn: 'root' })
export class HealthService {
  private api = inject(ApiService);

  getByAnimal(animalId?: string, type?: string): Observable<HealthRecord[]> {
    return this.api.get<HealthRecord[]>('/health', { animalId, type });
  }
  getReminders(daysAhead = 7, asOf?: string): Observable<DueReminder[]> {
    return this.api.get<DueReminder[]>('/health/reminders', { daysAhead, asOf });
  }
  create(req: CreateHealthRecordRequest): Observable<{ healthId: string }> {
    return this.api.post<{ healthId: string }>('/health', req);
  }
}
