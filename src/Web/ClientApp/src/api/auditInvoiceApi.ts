import { apiSlice } from './generated/apiSlice';
import type { PlatformReconciliationReportDto } from './generated/apiSlice';

export interface GenerateReconciliationReportArg {
  file: File;
  year: number;
  month: number;
  platformId: string;
}

// The auto-generated postApiReportsPlatformReconciliationGenerate/Export mutations describe
// their body as a plain JSON-ish shape (OpenAPI's IFormFile placeholder), which fetchBaseQuery
// would serialize as JSON — wrong for a multipart upload. Same fix as employeeDocumentsApi's
// uploadEmployeeDocument: build real FormData by hand instead of using the generated mutation.
function buildFormData({ file, year, month, platformId }: GenerateReconciliationReportArg): FormData {
  const formData = new FormData();
  formData.append('file', file);
  formData.append('year', String(year));
  formData.append('month', String(month));
  formData.append('platformId', platformId);
  return formData;
}

// Named for the backend command it calls (GenerateReconciliationReportCommand /
// /api/Reports/platform-reconciliation/...) rather than the "Audit Invoice" display name —
// only the tab/route/page-facing names changed, not the backend contract this wraps.
export const auditInvoiceApi = apiSlice.injectEndpoints({
  endpoints: (build) => ({
    generateReconciliationReport: build.mutation<PlatformReconciliationReportDto, GenerateReconciliationReportArg>({
      query: (arg) => ({
        url: '/api/Reports/platform-reconciliation/generate',
        method: 'POST',
        body: buildFormData(arg),
      }),
    }),
  }),
});

export const { useGenerateReconciliationReportMutation } = auditInvoiceApi;

/** Export goes through downloadFile/previewFile/printFile (raw file response, not JSON) — this
 * just builds the multipart body they POST, mirroring buildFormData above plus the format field. */
export function buildReconciliationExportFormData(
  arg: GenerateReconciliationReportArg & { format: 0 | 1 }
): FormData {
  const formData = buildFormData(arg);
  formData.append('format', String(arg.format));
  return formData;
}

export const RECONCILIATION_EXPORT_URL = '/api/Reports/platform-reconciliation/export';
