import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Animal, CreateAnimalRequest, UpdateAnimalRequest } from '../models';

@Injectable({ providedIn: 'root' })
export class AnimalService {
  private api = inject(ApiService);

  getAll(status?: string, search?: string): Observable<Animal[]> {
    return this.api.get<Animal[]>('/animals', { status, search });
  }
  getById(id: string): Observable<Animal> {
    return this.api.get<Animal>(`/animals/${id}`);
  }
  create(req: CreateAnimalRequest): Observable<{ animalId: string }> {
    return this.api.post<{ animalId: string }>('/animals', req);
  }
  update(id: string, req: UpdateAnimalRequest): Observable<void> {
    return this.api.put<void>(`/animals/${id}`, req);
  }
  delete(id: string): Observable<void> {
    return this.api.delete<void>(`/animals/${id}`);
  }
}
