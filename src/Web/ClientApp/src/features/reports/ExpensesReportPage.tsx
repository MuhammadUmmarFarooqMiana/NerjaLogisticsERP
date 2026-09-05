import { useState } from 'react';
import { Alert, Box, Chip, Grid, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import { EXPENSE_CATEGORIES } from '../expenses/expenseCategories';
import { useGetApiPlatformsQuery } from '../../api/generated/apiSlice';
import type { ExpensesReportRowDto } from '../../api/generated/apiSlice';
import { expensesReportExportUrl, useGetApiReportsExpensesQuery } from '../../api/reportsApi';
import { DataTable } from '../../components/shared/DataTable';
import { formatDate } from '../../lib/formatDate';
import { CURRENT_MONTH, CURRENT_YEAR, TODAY } from './reportConstants';
import { ReportActionButtons } from './ReportActionButtons';
import { ReportPeriodFilters } from './ReportPeriodFilters';

const CUSTOM = 3;

function SummaryTile({ label, value }: { label: string; value: string }) {
  return (
    <Grid size={{ xs: 12, sm: 6 }}>
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

export default function ExpensesReportPage() {
  const { t, i18n } = useTranslation(['reports', 'expenses']);

  const [periodType, setPeriodType] = useState<number>(0);
  const [date, setDate] = useState(TODAY);
  const [year, setYear] = useState(CURRENT_YEAR);
  const [month, setMonth] = useState(CURRENT_MONTH);
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [category, setCategory] = useState('');
  const [platformId, setPlatformId] = useState('');

  const { data: platforms = [] } = useGetApiPlatformsQuery();

  const isCustom = periodType === CUSTOM;
  const customReady = !isCustom || (!!startDate && !!endDate);

  const periodArgs = {
    periodType,
    date: periodType === 0 || periodType === 1 ? date : undefined,
    year: periodType === 2 ? year : undefined,
    month: periodType === 2 ? month : undefined,
    startDate: isCustom ? startDate : undefined,
    endDate: isCustom ? endDate : undefined,
    category: category === '' ? undefined : Number(category),
    platformId: platformId || undefined,
  };

  const { data: report, isLoading, error } = useGetApiReportsExpensesQuery(periodArgs, { skip: !customReady });

  const columns: ColumnDef<ExpensesReportRowDto, unknown>[] = [
    {
      accessorKey: 'category',
      header: t('columns.category'),
      cell: (info) => t(`expenses:categories.${info.getValue() as string}`),
    },
    {
      accessorKey: 'amount',
      header: t('columns.amount'),
      cell: (info) => `SAR ${Number(info.getValue()).toFixed(2)}`,
    },
    {
      accessorKey: 'expenseDate',
      header: t('columns.orderDate'),
      cell: (info) => formatDate(info.getValue() as string, i18n.language),
    },
    {
      accessorKey: 'platformName',
      header: t('columns.platform'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
    {
      accessorKey: 'description',
      header: t('columns.description'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
  ];

  return (
    <Box>
      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
        <Box>
          <Typography variant="h4" gutterBottom>
            {t('expenses.title')}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {t('expenses.subtitle')}
          </Typography>
        </Box>
        <ReportActionButtons
          disabled={!report}
          exportUrl={(format) => expensesReportExportUrl({ ...periodArgs, format })}
          fallbackFileName="expenses-report"
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
          label={t('expenses:filters.category')}
          value={category}
          onChange={(event) => setCategory(event.target.value)}
          sx={{ minWidth: 200 }}
        >
          <MenuItem value="">{t('expenses:filters.allCategories')}</MenuItem>
          {EXPENSE_CATEGORIES.map((option) => (
            <MenuItem key={option.value} value={option.value}>
              {t(`expenses:categories.${option.key}`)}
            </MenuItem>
          ))}
        </TextField>

        <TextField
          select
          size="small"
          label={t('filters.platform')}
          value={platformId}
          onChange={(event) => setPlatformId(event.target.value)}
          sx={{ minWidth: 180 }}
        >
          <MenuItem value="">{t('filters.allPlatforms')}</MenuItem>
          {platforms.map((platform) => (
            <MenuItem key={platform.id} value={platform.id}>
              {platform.name}
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
            <SummaryTile label={t('summary.totalAmount')} value={`SAR ${Number(report.totalAmount).toFixed(2)}`} />
            <SummaryTile label={t('summary.totalCount')} value={String(report.totalCount)} />
          </Grid>

          {report.byCategory && report.byCategory.length > 0 && (
            <Stack direction="row" spacing={1} sx={{ mb: 3, flexWrap: 'wrap' }}>
              {report.byCategory.map((c) => (
                <Chip
                  key={c.category}
                  label={`${t(`expenses:categories.${c.category}`)}: SAR ${Number(c.amount).toFixed(2)}`}
                />
              ))}
            </Stack>
          )}
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
