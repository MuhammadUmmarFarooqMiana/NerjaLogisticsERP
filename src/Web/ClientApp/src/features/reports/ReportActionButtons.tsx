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

const PDF = 0;
const EXCEL = 1;

interface ReportActionButtonsProps {
  /** True until a report has loaded — every action needs a report to export. */
  disabled: boolean;
  /** Builds the export URL for the given format (0 = Pdf, 1 = Excel) from the page's current filters. */
  exportUrl: (format: typeof PDF | typeof EXCEL) => string;
  fallbackFileName: string;
}

/** Preview/Print/Export PDF/Export Excel — identical wiring across every report, since they all
 * go through the same PDF/Excel export endpoint shape. Preview and Print both use the PDF
 * rendering, so what's previewed or printed always matches what Export PDF would download. */
export function ReportActionButtons({ disabled, exportUrl, fallbackFileName }: ReportActionButtonsProps) {
  const { t } = useTranslation('reports');
  const toast = useToast();
  const accessToken = useAppSelector(selectAccessToken);
  const [busyAction, setBusyAction] = useState<'preview' | 'print' | 'pdf' | 'excel' | null>(null);

  const handlePreview = async () => {
    setBusyAction('preview');
    try {
      await previewFile(exportUrl(PDF), accessToken);
    } catch {
      toast.error(t('previewError'));
    } finally {
      setBusyAction(null);
    }
  };

  const handlePrint = async () => {
    setBusyAction('print');
    try {
      await printFile(exportUrl(PDF), accessToken);
    } catch {
      toast.error(t('printError'));
    } finally {
      setBusyAction(null);
    }
  };

  const handleExport = async (format: typeof PDF | typeof EXCEL) => {
    setBusyAction(format === PDF ? 'pdf' : 'excel');
    try {
      await downloadFile(exportUrl(format), accessToken, fallbackFileName);
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
