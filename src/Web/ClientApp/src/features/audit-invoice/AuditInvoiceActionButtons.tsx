import { useState } from 'react';
import { Button, CircularProgress, Stack } from '@mui/material';
import GridOnOutlinedIcon from '@mui/icons-material/GridOnOutlined';
import PictureAsPdfOutlinedIcon from '@mui/icons-material/PictureAsPdfOutlined';
import PrintOutlinedIcon from '@mui/icons-material/PrintOutlined';
import VisibilityOutlinedIcon from '@mui/icons-material/VisibilityOutlined';
import { useTranslation } from 'react-i18next';
import { useAppSelector } from '../../app/hooks';
import { useToast } from '../../components/feedback/ToastContext';
import { downloadFile, previewFile, printFile } from '../../lib/downloadFile';
import { selectAccessToken } from '../auth/authSlice';
import { RECONCILIATION_EXPORT_URL, type GenerateReconciliationReportArg } from '../../api/auditInvoiceApi';

const PDF = 0;
const EXCEL = 1;

interface AuditInvoiceActionButtonsProps {
  disabled: boolean;
  /** The same filters (platform/year/month/file) the preview was generated from — export
   * re-parses the file server-side rather than relying on anything cached from Generate. */
  buildArg: () => GenerateReconciliationReportArg | null;
  fallbackFileName: string;
}

/** Same Preview/Print/Export PDF/Export Excel wiring as ReportActionButtons, but POSTing a
 * multipart body (the uploaded file + filters) instead of a GET query string, since this
 * report — uniquely among reports — takes a file upload. */
export function AuditInvoiceActionButtons({
  disabled,
  buildArg,
  fallbackFileName,
}: AuditInvoiceActionButtonsProps) {
  const { t } = useTranslation('reports');
  const toast = useToast();
  const accessToken = useAppSelector(selectAccessToken);
  const [busyAction, setBusyAction] = useState<'preview' | 'print' | 'pdf' | 'excel' | null>(null);

  const buildInit = (format: typeof PDF | typeof EXCEL): RequestInit | null => {
    const arg = buildArg();
    if (!arg) return null;
    const formData = new FormData();
    formData.append('file', arg.file);
    formData.append('year', String(arg.year));
    formData.append('month', String(arg.month));
    formData.append('platformId', arg.platformId);
    formData.append('format', String(format));
    return { method: 'POST', body: formData };
  };

  const handlePreview = async () => {
    const init = buildInit(PDF);
    if (!init) return;
    setBusyAction('preview');
    try {
      await previewFile(RECONCILIATION_EXPORT_URL, accessToken, init);
    } catch {
      toast.error(t('previewError'));
    } finally {
      setBusyAction(null);
    }
  };

  const handlePrint = async () => {
    const init = buildInit(PDF);
    if (!init) return;
    setBusyAction('print');
    try {
      await printFile(RECONCILIATION_EXPORT_URL, accessToken, init);
    } catch {
      toast.error(t('printError'));
    } finally {
      setBusyAction(null);
    }
  };

  const handleExport = async (format: typeof PDF | typeof EXCEL) => {
    const init = buildInit(format);
    if (!init) return;
    setBusyAction(format === PDF ? 'pdf' : 'excel');
    try {
      await downloadFile(RECONCILIATION_EXPORT_URL, accessToken, fallbackFileName, init);
    } catch {
      toast.error(t('exportError'));
    } finally {
      setBusyAction(null);
    }
  };

  return (
    <Stack direction="row" spacing={1} sx={{ flexWrap: 'wrap' }}>
      <Button
        variant="outlined"
        startIcon={busyAction === 'preview' ? <CircularProgress size={16} /> : <VisibilityOutlinedIcon />}
        disabled={disabled || busyAction !== null}
        onClick={() => void handlePreview()}
      >
        {t('preview')}
      </Button>
      <Button
        variant="outlined"
        startIcon={busyAction === 'print' ? <CircularProgress size={16} /> : <PrintOutlinedIcon />}
        disabled={disabled || busyAction !== null}
        onClick={() => void handlePrint()}
      >
        {t('print')}
      </Button>
      <Button
        variant="outlined"
        startIcon={busyAction === 'pdf' ? <CircularProgress size={16} /> : <PictureAsPdfOutlinedIcon />}
        disabled={disabled || busyAction !== null}
        onClick={() => void handleExport(PDF)}
      >
        {t('exportPdf')}
      </Button>
      <Button
        variant="outlined"
        startIcon={busyAction === 'excel' ? <CircularProgress size={16} /> : <GridOnOutlinedIcon />}
        disabled={disabled || busyAction !== null}
        onClick={() => void handleExport(EXCEL)}
      >
        {t('exportExcel')}
      </Button>
    </Stack>
  );
}
