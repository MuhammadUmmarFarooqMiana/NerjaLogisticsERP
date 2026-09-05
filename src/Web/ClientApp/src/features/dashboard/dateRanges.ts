// Formats using local calendar components — not toISOString(), which converts
// to UTC first and silently shifts the date back a day in any timezone ahead
// of UTC (e.g. local midnight Aug 1 in UTC+5 becomes "2026-07-31" in UTC).
export function toDateOnly(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

export function todayRange() {
  const today = toDateOnly(new Date());
  return { startDate: today, endDate: today };
}

export function currentMonthRange() {
  const now = new Date();
  const start = new Date(now.getFullYear(), now.getMonth(), 1);
  const end = new Date(now.getFullYear(), now.getMonth() + 1, 0);
  return { startDate: toDateOnly(start), endDate: toDateOnly(end) };
}

export function lastNDaysRange(days: number) {
  const end = new Date();
  const start = new Date();
  start.setDate(end.getDate() - (days - 1));
  return { startDate: toDateOnly(start), endDate: toDateOnly(end) };
}

/** Spans from the 1st of the month `months - 1` months ago through the end of the current month. */
export function lastNMonthsRange(months: number) {
  const now = new Date();
  const start = new Date(now.getFullYear(), now.getMonth() - (months - 1), 1);
  const end = new Date(now.getFullYear(), now.getMonth() + 1, 0);
  return { startDate: toDateOnly(start), endDate: toDateOnly(end) };
}

export const CURRENT_YEAR = new Date().getFullYear();
export const CURRENT_MONTH = new Date().getMonth() + 1;
