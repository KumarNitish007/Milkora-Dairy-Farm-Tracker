import { HttpContextToken, HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../services/notification.service';

/** Set on a request to suppress the error toast (e.g. optional desktop-only probes). */
export const SKIP_ERROR_TOAST = new HttpContextToken<boolean>(() => false);

/**
 * Surfaces API errors as toasts using the ApiResponse envelope's message /
 * field errors, then rethrows so callers can still react.
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notify = inject(NotificationService);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      // Silent for opt-out requests (desktop-only probes that 404 in the web build).
      if (req.context.get(SKIP_ERROR_TOAST)) return throwError(() => err);

      let message = 'Something went wrong. Please try again.';

      if (err.status === 0) {
        message = 'Cannot reach the server. Check your connection.';
      } else if (err.error?.errors) {
        // Validation errors: show the first field message.
        const first = Object.values(err.error.errors as Record<string, string[]>)[0];
        message = first?.[0] ?? err.error.message ?? message;
      } else if (err.error?.message) {
        message = err.error.message;
      } else if (typeof err.error === 'string' && err.error) {
        message = err.error;
      }

      notify.error(message);
      return throwError(() => err);
    }),
  );
};
