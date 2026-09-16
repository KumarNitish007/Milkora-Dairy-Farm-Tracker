/** yyyy-MM-dd for a Date (local), suitable for <input type="date"> and API. */
export function toIso(d: Date): string {
  const tz = d.getTimezoneOffset() * 60000;
  return new Date(d.getTime() - tz).toISOString().slice(0, 10);
}

export function today(): string {
  return toIso(new Date());
}

export function daysAgo(n: number): string {
  const d = new Date();
  d.setDate(d.getDate() - n);
  return toIso(d);
}

export function monthStart(): string {
  const d = new Date();
  return toIso(new Date(d.getFullYear(), d.getMonth(), 1));
}
