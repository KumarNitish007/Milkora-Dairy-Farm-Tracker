/** Mirror of the API's unified envelope. */
export interface ApiResponse<T = unknown> {
  success: boolean;
  message?: string | null;
  data?: T;
  errors?: Record<string, string[]> | null;
}
