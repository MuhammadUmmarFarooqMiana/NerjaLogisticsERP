import { useState } from 'react';
import { Alert, Box, Chip, Grid, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import { useGetApiEmployeesQuery } from '../../api/employeesApi';
import { useGetApiPlatformsQuery } from '../../api/generated/apiSlice';
import type { OrdersReportRowDto } from '../../api/generated/apiSlice';
import { ordersReportExportUrl, useGetApiReportsOrdersQuery } from '../../api/reportsApi';
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

export default function OrdersReportPage() {
  const { t, i18n } = useTranslation('reports');

  const [periodType, setPeriodType] = useState<number>(0);
  const [date, setDate] = useState(TODAY);
  const [year, setYear] = useState(CURRENT_YEAR);
  const [month, setMonth] = useState(CURRENT_MONTH);
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [platformId, setPlatformId] = useState('');
  const [employeeId, setEmployeeId] = useState('');

  const { data: platforms = [] } = useGetApiPlatformsQuery();
  const { data: employees } = useGetApiEmployeesQuery({});

  const isCustom = periodType === CUSTOM;
  const customReady = !isCustom || (!!startDate && !!endDate);

  const periodArgs = {
    periodType,
    date: periodType === 0 || periodType === 1 ? date : undefined,
    year: periodType === 2 ? year : undefined,
    month: periodType === 2 ? month : undefined,
    startDate: isCustom ? startDate : undefined,
    endDate: isCustom ? endDate : undefined,
    platformId: platformId || undefined,
    employeeId: employeeId || undefined,
  };

  const { data: report, isLoading, error } = useGetApiReportsOrdersQuery(periodArgs, { skip: !customReady });

  const columns: ColumnDef<OrdersReportRowDto, unknown>[] = [
    { accessorKey: 'employeeName', header: t('columns.employee') },
    {
      accessorKey: 'platformName',
      header: t('columns.platform'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
    {
      accessorKey: 'orderDate',
      header: t('columns.orderDate'),
      cell: (info) => formatDate(info.getValue() as string, i18n.language),
    },
    { accessorKey: 'completedOrders', header: t('columns.completedOrders') },
  ];

  return (
    <Box>
      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
        <Box>
          <Typography variant="h4" gutterBottom>
            {t('orders.title')}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {t('orders.subtitle')}
          </Typography>
        </Box>
        <ReportActionButtons
          disabled={!report}
          exportUrl={(format) => ordersReportExportUrl({ ...periodArgs, format })}
          fallbackFileName="orders-report"
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

        <TextField
          select
          size="small"
          label={t('filters.employee')}
          value={employeeId}
          onChange={(event) => setEmployeeId(event.target.value)}
          sx={{ minWidth: 200 }}
        >
          <MenuItem value="">{t('filters.allEmployees')}</MenuItem>
          {(employees ?? []).map((employee) => (
            <MenuItem key={employee.id} value={employee.id}>
              {employee.fullName}
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
            <SummaryTile label={t('summary.totalCompletedOrders')} value={String(report.totalCompletedOrders)} />
            <SummaryTile label={t('summary.totalSessions')} value={String(report.totalSessions)} />
            <SummaryTile label={t('summary.uniqueRiders')} value={String(report.uniqueRiders)} />
          </Grid>

          {report.byPlatform && report.byPlatform.length > 0 && (
            <Stack direction="row" spacing={1} sx={{ mb: 3, flexWrap: 'wrap' }}>
              {report.byPlatform.map((platform) => (
                <Chip key={platform.platformName} label={`${platform.platformName}: ${platform.completedOrders}`} />
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
