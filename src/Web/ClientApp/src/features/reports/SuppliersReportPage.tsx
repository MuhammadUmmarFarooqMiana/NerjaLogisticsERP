import { useState } from 'react';
import { Alert, Box, Grid, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import { useGetApiSuppliersQuery } from '../../api/suppliersApi';
import type { SuppliersReportRowDto } from '../../api/generated/apiSlice';
import { suppliersReportExportUrl, useGetApiReportsSuppliersQuery } from '../../api/reportsApi';
import { DataTable } from '../../components/shared/DataTable';
import { formatDate } from '../../lib/formatDate';
import { CURRENT_MONTH, CURRENT_YEAR, TODAY } from './reportConstants';
import { ReportActionButtons } from './ReportActionButtons';
import { ReportPeriodFilters } from './ReportPeriodFilters';

const CUSTOM = 3;

function SummaryTile({ label, value }: { label: string; value: string }) {
  return (
    <Grid size={{ xs: 12, sm: 4 }}>
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

export default function SuppliersReportPage() {
  const { t, i18n } = useTranslation('reports');

  const [periodType, setPeriodType] = useState<number>(0);
  const [date, setDate] = useState(TODAY);
  const [year, setYear] = useState(CURRENT_YEAR);
  const [month, setMonth] = useState(CURRENT_MONTH);
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [supplierId, setSupplierId] = useState('');

  const { data: suppliers } = useGetApiSuppliersQuery({});

  const isCustom = periodType === CUSTOM;
  const customReady = !isCustom || (!!startDate && !!endDate);

  const periodArgs = {
    periodType,
    date: periodType === 0 || periodType === 1 ? date : undefined,
    year: periodType === 2 ? year : undefined,
    month: periodType === 2 ? month : undefined,
    startDate: isCustom ? startDate : undefined,
    endDate: isCustom ? endDate : undefined,
    supplierId: supplierId || undefined,
  };

  const { data: report, isLoading, error } = useGetApiReportsSuppliersQuery(periodArgs, { skip: !customReady });

  const columns: ColumnDef<SuppliersReportRowDto, unknown>[] = [
    { accessorKey: 'supplierName', header: t('suppliers.columns.supplier') },
    { accessorKey: 'itemName', header: t('suppliers.columns.item') },
    { accessorKey: 'quantity', header: t('suppliers.columns.quantity') },
    {
      accessorKey: 'stockDate',
      header: t('columns.orderDate'),
      cell: (info) => formatDate(info.getValue() as string, i18n.language),
    },
  ];

  return (
    <Box>
      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
        <Box>
          <Typography variant="h4" gutterBottom>
            {t('suppliers.title')}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {t('suppliers.subtitle')}
          </Typography>
        </Box>
        <ReportActionButtons
          disabled={!report}
          exportUrl={(format) => suppliersReportExportUrl({ ...periodArgs, format })}
          fallbackFileName="suppliers-report"
        />
      </Stack>

      <Stack direction="row" spacing={2} sx={{ my: 2, flexWrap: 'wrap' }}>
        <ReportPeriodFilters
          periodType={periodType}
          onPeriodTypeChange={setPeriodType}
          date={date}
          onDateChange={setDate}
          year={year}
          onYearChange={setYear}
          month={month}
          onMonthChange={setMonth}
          startDate={startDate}
          onStartDateChange={setStartDate}
          endDate={endDate}
          onEndDateChange={setEndDate}
        />

        <TextField
          select
          size="small"
          label={t('suppliers.filters.supplier')}
          value={supplierId}
          onChange={(event) => setSupplierId(event.target.value)}
          sx={{ minWidth: 200 }}
        >
          <MenuItem value="">{t('suppliers.filters.allSuppliers')}</MenuItem>
          {(suppliers ?? []).map((supplier) => (
            <MenuItem key={supplier.id} value={supplier.id}>
              {supplier.name}
            </MenuItem>
          ))}
        </TextField>
      </Stack>

      {isCustom && !customReady && <Alert severity="info" sx={{ mb: 2 }}>{t('customDateHint')}</Alert>}

      {report && (
        <>
          <Typography variant="subtitle1" sx={{ mb: 2 }}>
            {report.periodLabel}
          </Typography>
          <Grid container spacing={2} sx={{ mb: 3 }}>
            <SummaryTile label={t('suppliers.summary.totalSuppliers')} value={String(report.totalSuppliers)} />
            <SummaryTile label={t('suppliers.summary.totalTransactions')} value={String(report.totalTransactions)} />
            <SummaryTile
              label={t('suppliers.summary.totalQuantityReceived')}
              value={String(report.totalQuantityReceived)}
            />
          </Grid>
        </>
      )}

      <DataTable
        columns={columns}
        data={report?.rows ?? []}
        isLoading={isLoading}
        error={error}
        emptyMessage={t('empty')}
      />
    </Box>
  );
}
