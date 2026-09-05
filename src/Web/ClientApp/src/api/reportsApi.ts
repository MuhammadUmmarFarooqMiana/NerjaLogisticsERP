import { apiSlice } from './generated/apiSlice';

export const {
  useGetApiReportsOrdersQuery,
  useGetApiReportsFinesQuery,
  useGetApiReportsAdvancesQuery,
  useGetApiReportsExpensesQuery,
  useGetApiReportsLeavesQuery,
  useGetApiReportsVehiclesQuery,
  useGetApiReportsSuppliersQuery,
  useGetApiReportsInventoryLedgerQuery,
  useGetApiReportsSalariesQuery,
} = apiSlice;

interface BaseReportParams {
  periodType: number;
  format: number;
  date?: string;
  year?: number;
  month?: number;
  startDate?: string;
  endDate?: string;
}

/** The export response is a raw file, not JSON, so it's fetched via downloadFile/previewFile/
 * printFile rather than through an RTK Query hook — this builds the query string they share. */
function baseReportQuery(params: BaseReportParams): URLSearchParams {
  const query = new URLSearchParams();
  query.set('periodType', String(params.periodType));
  query.set('format', String(params.format));
  if (params.date) query.set('date', params.date);
  if (params.year != null) query.set('year', String(params.year));
  if (params.month != null) query.set('month', String(params.month));
  if (params.startDate) query.set('startDate', params.startDate);
  if (params.endDate) query.set('endDate', params.endDate);
  return query;
}

export function ordersReportExportUrl(
  params: BaseReportParams & { platformId?: string; employeeId?: string }
): string {
  const query = baseReportQuery(params);
  if (params.platformId) query.set('platformId', params.platformId);
  if (params.employeeId) query.set('employeeId', params.employeeId);
  return `/api/Reports/orders/export?${query.toString()}`;
}

export function finesReportExportUrl(params: BaseReportParams & { employeeId?: string }): string {
  const query = baseReportQuery(params);
  if (params.employeeId) query.set('employeeId', params.employeeId);
  return `/api/Reports/fines/export?${query.toString()}`;
}

export function advancesReportExportUrl(params: BaseReportParams & { employeeId?: string }): string {
  const query = baseReportQuery(params);
  if (params.employeeId) query.set('employeeId', params.employeeId);
  return `/api/Reports/advances/export?${query.toString()}`;
}

export function expensesReportExportUrl(
  params: BaseReportParams & { category?: number; platformId?: string }
): string {
  const query = baseReportQuery(params);
  if (params.category != null) query.set('category', String(params.category));
  if (params.platformId) query.set('platformId', params.platformId);
  return `/api/Reports/expenses/export?${query.toString()}`;
}

export function leavesReportExportUrl(
  params: BaseReportParams & { status?: number; employeeId?: string }
): string {
  const query = baseReportQuery(params);
  if (params.status != null) query.set('status', String(params.status));
  if (params.employeeId) query.set('employeeId', params.employeeId);
  return `/api/Reports/leaves/export?${query.toString()}`;
}

export function vehiclesReportExportUrl(params: BaseReportParams & { vehicleId?: string }): string {
  const query = baseReportQuery(params);
  if (params.vehicleId) query.set('vehicleId', params.vehicleId);
  return `/api/Reports/vehicles/export?${query.toString()}`;
}

export function suppliersReportExportUrl(params: BaseReportParams & { supplierId?: string }): string {
  const query = baseReportQuery(params);
  if (params.supplierId) query.set('supplierId', params.supplierId);
  return `/api/Reports/suppliers/export?${query.toString()}`;
}

export function inventoryLedgerReportExportUrl(params: BaseReportParams & { itemId?: string }): string {
  const query = baseReportQuery(params);
  if (params.itemId) query.set('itemId', params.itemId);
  return `/api/Reports/inventory-ledger/export?${query.toString()}`;
}

export function salariesReportExportUrl(
  params: BaseReportParams & { status?: number; employeeId?: string }
): string {
  const query = baseReportQuery(params);
  if (params.status != null) query.set('status', String(params.status));
  if (params.employeeId) query.set('employeeId', params.employeeId);
  return `/api/Reports/salaries/export?${query.toString()}`;
}
