import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { BreedingRecord, CreateBreedingRequest, UpdateCalvingRequest } from '../models';

@Injectable({ providedIn: 'root' })
export class BreedingService {
  private api = inject(ApiService);

  getByAnimal(animalId?: string): Observable<BreedingRecord[]> {
    return this.api.get<BreedingRecord[]>('/breeding', { animalId });
  }
  create(req: CreateBreedingRequest): Observable<{ breedingId: string }> {
    return this.api.post<{ breedingId: string }>('/breeding', req);
  }
  updateCalving(id: string, req: UpdateCalvingRequest): Observable<void> {
    return this.api.put<void>(`/breeding/${id}/calving`, req);
  }
}
