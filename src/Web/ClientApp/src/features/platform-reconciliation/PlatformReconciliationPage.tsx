import { useRef, useState } from 'react';
import { Alert, Box, Button, Grid, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material';
import CloudUploadOutlinedIcon from '@mui/icons-material/CloudUploadOutlined';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import { useGetApiPlatformsQuery } from '../../api/generated/apiSlice';
import type {
  PlatformReconciliationMissingFromSheetDto,
  PlatformReconciliationRowDto,
  PlatformReconciliationUnmatchedPlatformRowDto,
} from '../../api/generated/apiSlice';
import { useGenerateReconciliationReportMutation, type GenerateReconciliationReportArg } from '../../api/platformReconciliationApi';
import { DataTable } from '../../components/shared/DataTable';
import { getApiErrorMessages } from '../../lib/apiError';
import { CURRENT_MONTH, CURRENT_YEAR } from '../reports/reportConstants';
import { PlatformReconciliationActionButtons } from './PlatformReconciliationActionButtons';

const MONTHS = Array.from({ length: 12 }, (_, i) => i + 1);

function SummaryTile({ label, value }: { label: string; value: string }) {
  return (
    <Grid size={{ xs: 12, sm: 6, md: 3 }}>
      <Paper variant="outlined" sx={{ p: 2.5 }}>
        <Typography variant="body2" color="text.secondary" noWrap>
          {label}
        </Typography>
        <Typography variant="h5" sx={{ fontWeight: 700, mt: 0.5 }}>
          {value}
        </Typography>
      </Paper>
    </Grid>
  );
}

export default function PlatformReconciliationPage() {
  const { t } = useTranslation('platformReconciliation');
  const fileInputRef = useRef<HTMLInputElement | null>(null);

  const { data: platforms } = useGetApiPlatformsQuery();
  const [platformId, setPlatformId] = useState('');
  const [year, setYear] = useState(CURRENT_YEAR);
  const [month, setMonth] = useState(CURRENT_MONTH);
  const [file, setFile] = useState<File | null>(null);
  const [formError, setFormError] = useState<string | null>(null);

  const [generate, { data: report, isLoading, error }] = useGenerateReconciliationReportMutation();

  const currentArg = (): GenerateReconciliationReportArg | null =>
    file && platformId ? { file, year, month, platformId } : null;

  const handleGenerate = async () => {
    setFormError(null);
    if (!platformId) {
      setFormError(t('errors.platformRequired'));
      return;
    }
    if (!file) {
      setFormError(t('errors.fileRequired'));
      return;
    }
    try {
      await generate({ file, year, month, platformId }).unwrap();
    } catch {
      // Surfaced below via `error` from the mutation hook.
    }
  };

  const matchedColumns: ColumnDef<PlatformReconciliationRowDto, unknown>[] = [
    { accessorKey: 'employeeName', header: t('columns.employee') },
    { accessorKey: 'platformIdNumber', header: t('columns.platformIdNumber') },
    { accessorKey: 'nerjaCompletedOrders', header: t('columns.nerjaOrders') },
    { accessorKey: 'platformCompletedOrders', header: t('columns.platformOrders') },
    {
      accessorKey: 'ordersDifference',
      header: t('columns.ordersDifference'),
      cell: (info) => {
        const value = Number(info.getValue());
        return (
          <Box component="span" sx={{ color: value === 0 ? 'text.primary' : '#D32F2F', fontWeight: value === 0 ? 400 : 600 }}>
            {value > 0 ? `+${value}` : value}
          </Box>
        );
      },
    },
    {
      accessorKey: 'totalPenalties',
      header: t('columns.totalPenalties'),
      cell: (info) => `SAR ${Number(info.getValue()).toFixed(2)}`,
    },
    {
      accessorKey: 'originalNetPayable',
      header: t('columns.originalNetPayable'),
      cell: (info) => `SAR ${Number(info.getValue()).toFixed(2)}`,
    },
    {
      accessorKey: 'adjustedNetPayable',
      header: t('columns.adjustedNetPayable'),
      cell: (info) => `SAR ${Number(info.getValue()).toFixed(2)}`,
    },
    { accessorKey: 'monthlySummaryStatus', header: t('columns.status') },
  ];

  const unmatchedColumns: ColumnDef<PlatformReconciliationUnmatchedPlatformRowDto, unknown>[] = [
    { accessorKey: 'platformIdNumber', header: t('columns.platformIdNumber') },
    { accessorKey: 'platformCompletedOrders', header: t('columns.platformOrders') },
    {
      accessorKey: 'totalPenalties',
      header: t('columns.totalPenalties'),
      cell: (info) => `SAR ${Number(info.getValue()).toFixed(2)}`,
    },
  ];

  const missingColumns: ColumnDef<PlatformReconciliationMissingFromSheetDto, unknown>[] = [
    { accessorKey: 'employeeName', header: t('columns.employee') },
    { accessorKey: 'platformIdNumber', header: t('columns.platformIdNumber') },
    { accessorKey: 'nerjaCompletedOrders', header: t('columns.nerjaOrders') },
    {
      accessorKey: 'netPayable',
      header: t('columns.originalNetPayable'),
      cell: (info) => `SAR ${Number(info.getValue()).toFixed(2)}`,
    },
  ];

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('title')}
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        {t('subtitle')}
      </Typography>

      <Paper variant="outlined" sx={{ p: 3, mb: 3 }}>
        <Stack direction="row" spacing={2} sx={{ flexWrap: 'wrap', alignItems: 'flex-start' }}>
          <TextField
            select
            size="small"
            label={t('form.platform')}
            value={platformId}
            onChange={(event) => setPlatformId(event.target.value)}
            sx={{ minWidth: 180 }}
          >
            {(platforms ?? []).map((platform) => (
              <MenuItem key={platform.id} value={platform.id}>
                {platform.name}
              </MenuItem>
            ))}
          </TextField>

          <TextField
            select
            size="small"
            label={t('form.year')}
            value={year}
            onChange={(event) => setYear(Number(event.target.value))}
            sx={{ width: 120 }}
          >
            {[CURRENT_YEAR - 1, CURRENT_YEAR, CURRENT_YEAR + 1].map((y) => (
              <MenuItem key={y} value={y}>
                {y}
              </MenuItem>
            ))}
          </TextField>

          <TextField
            select
            size="small"
            label={t('form.month')}
            value={month}
            onChange={(event) => setMonth(Number(event.target.value))}
            sx={{ width: 160 }}
          >
            {MONTHS.map((m) => (
              <MenuItem key={m} value={m}>
                {new Date(2000, m - 1, 1).toLocaleDateString(undefined, { month: 'long' })}
              </MenuItem>
            ))}
          </TextField>

          <Stack spacing={0.5}>
            <input
              ref={fileInputRef}
              type="file"
              accept=".xlsx"
              hidden
              onChange={(event) => setFile(event.target.files?.[0] ?? null)}
            />
            <Button
              variant="outlined"
              size="small"
              startIcon={<CloudUploadOutlinedIcon fontSize="small" />}
              onClick={() => fileInputRef.current?.click()}
            >
              {file ? t('form.replaceFile') : t('form.chooseFile')}
            </Button>
            <Typography variant="caption" color="text.secondary" noWrap sx={{ maxWidth: 220 }}>
              {file ? file.name : t('form.noFileChosen')}
            </Typography>
          </Stack>

          <Button
            variant="contained"
            disabled={isLoading}
            onClick={() => void handleGenerate()}
            sx={{ height: 40 }}
          >
            {isLoading ? t('form.generating') : t('form.generate')}
          </Button>
        </Stack>

        {formError && (
          <Alert severity="warning" sx={{ mt: 2 }}>
            {formError}
          </Alert>
        )}
        {error && (
          <Alert severity="error" sx={{ mt: 2 }}>
            {getApiErrorMessages(error, t('errors.generic')).join('\n')}
          </Alert>
        )}
      </Paper>

      {!report && !isLoading && <Alert severity="info">{t('empty')}</Alert>}

      {report && (
        <>
          <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'flex-start', mb: 2, flexWrap: 'wrap', gap: 2 }}>
            <Typography variant="subtitle1">
              {report.platformName} — {report.periodLabel}
            </Typography>
            <PlatformReconciliationActionButtons
              disabled={!report}
              buildArg={currentArg}
              fallbackFileName="platform-reconciliation"
            />
          </Stack>

          <Grid container spacing={2} sx={{ mb: 3 }}>
            <SummaryTile label={t('summary.matched')} value={String(report.totalMatched)} />
            <SummaryTile label={t('summary.ordersMismatch')} value={String(report.totalOrdersMismatchCount)} />
            <SummaryTile label={t('summary.unmatchedInPlatform')} value={String(report.totalUnmatchedInPlatform)} />
            <SummaryTile label={t('summary.missingFromSheet')} value={String(report.totalMissingFromSheet)} />
          </Grid>
          <Grid container spacing={2} sx={{ mb: 4 }}>
            <SummaryTile label={t('summary.totalPenalties')} value={`SAR ${Number(report.totalPenalties ?? 0).toFixed(2)}`} />
            <SummaryTile label={t('summary.totalOriginalNet')} value={`SAR ${Number(report.totalOriginalNetPayable ?? 0).toFixed(2)}`} />
            <SummaryTile label={t('summary.totalAdjustedNet')} value={`SAR ${Number(report.totalAdjustedNetPayable ?? 0).toFixed(2)}`} />
          </Grid>

          <Typography variant="h6" sx={{ mb: 0.5 }}>
            {t('sections.matched.title')}
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 1.5 }}>
            {t('sections.matched.description')}
          </Typography>
          <DataTable columns={matchedColumns} data={report.matchedRows ?? []} isLoading={false} />

          <Typography variant="h6" sx={{ mt: 4, mb: 0.5 }}>
            {t('sections.unmatchedInPlatform.title')}
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 1.5 }}>
            {t('sections.unmatchedInPlatform.description')}
          </Typography>
          <DataTable
            columns={unmatchedColumns}
            data={report.unmatchedPlatformRows ?? []}
            isLoading={false}
            emptyMessage={t('sections.unmatchedInPlatform.empty')}
          />

          <Typography variant="h6" sx={{ mt: 4, mb: 0.5 }}>
            {t('sections.missingFromSheet.title')}
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 1.5 }}>
            {t('sections.missingFromSheet.description')}
          </Typography>
          <DataTable
            columns={missingColumns}
            data={report.missingFromSheetRows ?? []}
            isLoading={false}
            emptyMessage={t('sections.missingFromSheet.empty')}
          />
        </>
      )}
    </Box>
  );
}
