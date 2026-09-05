import type {
  DailyOrderListItemDto,
  EmployeeListItemDto,
  ExpenseDto,
  MonthlySummaryDto,
  PlatformDto,
} from '../../api/generated/apiSlice';
import type { MonthlyAmountPoint } from '../../components/dashboard/MonthlyAmountTrendChart';
import type { OrdersTrendPoint } from '../../components/dashboard/OrdersTrendChart';
import type { StatusSlice } from '../../components/dashboard/EmployeeStatusChart';
import type { PlatformRoleCounts } from '../../components/dashboard/RidersByPlatformCard';
import { Roles } from '../../lib/roles';
import { lastNDaysRange, toDateOnly } from './dateRanges';

export function sumCompletedOrders(orders: DailyOrderListItemDto[]): number {
  return orders.reduce((sum, o) => sum + Number(o.completedOrders ?? 0), 0);
}

/** Buckets completed orders per calendar day across the last `days` days, filling in zero-order days. */
export function buildOrdersTrend(
  orders: DailyOrderListItemDto[],
  days: number,
  locale: string
): OrdersTrendPoint[] {
  const { startDate } = lastNDaysRange(days);
  const totalsByDate = new Map<string, number>();
  orders.forEach((o) => {
    if (!o.orderDate) return;
    totalsByDate.set(o.orderDate, (totalsByDate.get(o.orderDate) ?? 0) + Number(o.completedOrders ?? 0));
  });

  // Parse as local-date components (not `new Date(startDate)`, which reads a
  // bare "YYYY-MM-DD" as UTC midnight and drifts the label a day off in any
  // timezone behind UTC) so date-key lookups and displayed labels agree.
  const [startYear, startMonth, startDay] = startDate.split('-').map(Number);
  const cursor = new Date(startYear, startMonth - 1, startDay);

  const points: OrdersTrendPoint[] = [];
  for (let i = 0; i < days; i++) {
    const iso = toDateOnly(cursor);
    points.push({
      date: iso,
      label: cursor.toLocaleDateString(locale, { month: 'short', day: 'numeric' }),
      completedOrders: totalsByDate.get(iso) ?? 0,
    });
    cursor.setDate(cursor.getDate() + 1);
  }
  return points;
}

const ACCOUNT_STATUSES = ['Active', 'PendingReview', 'Rejected', 'Incomplete', 'Suspended', 'Terminated'] as const;

export function buildEmployeeStatusBreakdown(employees: EmployeeListItemDto[]): StatusSlice[] {
  const counts = new Map<string, number>();
  employees.forEach((e) => {
    if (!e.accountStatus) return;
    counts.set(e.accountStatus, (counts.get(e.accountStatus) ?? 0) + 1);
  });
  return ACCOUNT_STATUSES.filter((status) => counts.has(status)).map((status) => ({
    status,
    count: counts.get(status) ?? 0,
  }));
}

interface RoleBucket {
  rider: number;
  accountant: number;
  supervisor: number;
}

/** Per-platform headcount broken down by role (Rider/Accountant/Supervisor) — a
 * platform row is included whenever it has any employee at all, even if every
 * one of them falls outside those three roles (e.g. an Administrator), so its
 * counts would just read 0/0/0 rather than the platform silently disappearing. */
export function buildRolesByPlatform(
  employees: EmployeeListItemDto[],
  platforms: PlatformDto[],
  unassignedLabel: string
): PlatformRoleCounts[] {
  const buckets = new Map<string, RoleBucket>();
  employees.forEach((e) => {
    const key = e.platformName ?? unassignedLabel;
    const bucket = buckets.get(key) ?? { rider: 0, accountant: 0, supervisor: 0 };
    const roles = e.roles ?? [];
    if (roles.includes(Roles.Rider)) bucket.rider += 1;
    if (roles.includes(Roles.Accountant)) bucket.accountant += 1;
    if (roles.includes(Roles.Supervisor)) bucket.supervisor += 1;
    buckets.set(key, bucket);
  });

  const toRow = (platformId: string | null, platformName: string, bucket: RoleBucket): PlatformRoleCounts => ({
    platformId,
    platformName,
    riderCount: bucket.rider,
    accountantCount: bucket.accountant,
    supervisorCount: bucket.supervisor,
  });

  const result: PlatformRoleCounts[] = platforms
    .filter((p) => p.name && buckets.has(p.name))
    .map((p) => toRow(p.id ?? null, p.name!, buckets.get(p.name!)!));

  const unassignedBucket = buckets.get(unassignedLabel);
  if (unassignedBucket) {
    result.push(toRow(null, unassignedLabel, unassignedBucket));
  }

  return result;
}

/** Turns a "YYYY-MM" -> total map into `months` trailing points ending this calendar month,
 * filling in zero for any month with no entry — shared by every monthly-amount trend
 * (Expenses, Salary Paid, ...) so each only has to build the totals map. */
function fillTrailingMonths(totalsByMonth: Map<string, number>, months: number, locale: string): MonthlyAmountPoint[] {
  const now = new Date();
  const points: MonthlyAmountPoint[] = [];
  for (let i = months - 1; i >= 0; i--) {
    const cursor = new Date(now.getFullYear(), now.getMonth() - i, 1);
    const key = `${cursor.getFullYear()}-${String(cursor.getMonth() + 1).padStart(2, '0')}`;
    points.push({
      month: key,
      label: cursor.toLocaleDateString(locale, { month: 'short', year: '2-digit' }),
      amount: totalsByMonth.get(key) ?? 0,
    });
  }
  return points;
}

/** Buckets expense amounts per calendar month across the last `months` months, filling in zero months. */
export function buildExpensesByMonth(expenses: ExpenseDto[], months: number, locale: string): MonthlyAmountPoint[] {
  const totalsByMonth = new Map<string, number>();
  expenses.forEach((e) => {
    if (!e.expenseDate) return;
    const key = e.expenseDate.slice(0, 7); // "YYYY-MM"
    totalsByMonth.set(key, (totalsByMonth.get(key) ?? 0) + Number(e.amount ?? 0));
  });
  return fillTrailingMonths(totalsByMonth, months, locale);
}

/** Buckets Paid monthly summaries' net payable per calendar month across the last `months`
 * months — Year/Month are separate ints on MonthlySummaryDto (no date string to slice),
 * so the key is built from those directly instead. */
export function buildSalaryPaidByMonth(summaries: MonthlySummaryDto[], months: number, locale: string): MonthlyAmountPoint[] {
  const totalsByMonth = new Map<string, number>();
  summaries.forEach((s) => {
    if (!s.year || !s.month) return;
    const key = `${s.year}-${String(s.month).padStart(2, '0')}`;
    totalsByMonth.set(key, (totalsByMonth.get(key) ?? 0) + Number(s.netSalaryPayable ?? 0));
  });
  return fillTrailingMonths(totalsByMonth, months, locale);
}
