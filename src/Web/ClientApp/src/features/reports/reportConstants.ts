// Mirrors the backend enums' declaration order exactly (Application/Reports/Common) — enums
// serialize as their underlying int over the wire, so the numeric value here IS the contract.
export const REPORT_PERIOD_TYPES = [
  { value: 0, key: 'Daily' },
  { value: 1, key: 'Weekly' },
  { value: 2, key: 'Monthly' },
  { value: 3, key: 'Custom' },
] as const;

export const REPORT_FORMATS = [
  { value: 0, key: 'Pdf' },
  { value: 1, key: 'Excel' },
] as const;

export function toDateOnly(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

export const TODAY = toDateOnly(new Date());
export const CURRENT_YEAR = new Date().getFullYear();
export const CURRENT_MONTH = new Date().getMonth() + 1;
