// DateOnly values ("2026-08-11") carry no time component — Intl parses them
// as UTC midnight, so formatting must stay pinned to UTC or the displayed day
// shifts by one in negative-UTC-offset browsers. DateTimeOffset values (real
// timestamps, e.g. ProfileSubmittedAt) intentionally use the viewer's local
// time zone instead, since that's what "when did this happen" should show.
export function formatDate(value: string | null | undefined, locale: string): string {
  if (!value) return '';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;
  return new Intl.DateTimeFormat(locale, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    timeZone: 'UTC',
  }).format(date);
}

export function formatDateTime(value: string | null | undefined, locale: string): string {
  if (!value) return '';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;
  return new Intl.DateTimeFormat(locale, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  }).format(date);
}
