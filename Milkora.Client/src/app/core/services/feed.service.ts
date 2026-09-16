import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { FeedItem, CreateFeedItemRequest, UpdateStockRequest } from '../models';

@Injectable({ providedIn: 'root' })
export class FeedService {
  private api = inject(ApiService);

  getAll(search?: string): Observable<FeedItem[]> {
    return this.api.get<FeedItem[]>('/feed', { search });
  }
  getLowStock(): Observable<FeedItem[]> {
    return this.api.get<FeedItem[]>('/feed/lowstock');
  }
  create(req: CreateFeedItemRequest): Observable<{ inventoryId: string }> {
    return this.api.post<{ inventoryId: string }>('/feed', req);
  }
  updateStock(id: string, req: UpdateStockRequest): Observable<void> {
    return this.api.put<void>(`/feed/${id}/stock`, req);
  }
}
