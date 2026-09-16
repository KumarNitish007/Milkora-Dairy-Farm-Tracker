import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models';

/**
 * Thin HTTP layer that talks to the Milkora API and unwraps the ApiResponse<T>
 * envelope, so feature services receive the payload directly. HTTP-level errors
 * (400/404/409/500) are surfaced by the error interceptor.
 */
@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiBaseUrl;

  get<T>(url: string, params?: Record<string, unknown>): Observable<T> {
    return this.http
      .get<ApiResponse<T>>(this.base + url, { params: toParams(params) })
      .pipe(map(unwrap));
  }

  post<T>(url: string, body: unknown): Observable<T> {
    return this.http.post<ApiResponse<T>>(this.base + url, body).pipe(map(unwrap));
  }

  put<T>(url: string, body: unknown): Observable<T> {
    return this.http.put<ApiResponse<T>>(this.base + url, body).pipe(map(unwrap));
  }

  delete<T>(url: string): Observable<T> {
    return this.http.delete<ApiResponse<T>>(this.base + url).pipe(map(unwrap));
  }
}

function unwrap<T>(res: ApiResponse<T>): T {
  if (res && res.success === false) {
    throw new Error(res.message ?? 'Request failed.');
  }
  return (res?.data as T);
}

function toParams(params?: Record<string, unknown>): HttpParams {
  let hp = new HttpParams();
  if (!params) return hp;
  for (const [k, v] of Object.entries(params)) {
    if (v !== null && v !== undefined && v !== '') hp = hp.set(k, String(v));
  }
  return hp;
}
