import { useState } from 'react';
import { Alert, Box, Grid, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import { useGetApiEmployeesQuery } from '../../api/employeesApi';
import type { SalariesReportRowDto } from '../../api/generated/apiSlice';
import { salariesReportExportUrl, useGetApiReportsSalariesQuery } from '../../api/reportsApi';
import { DataTable } from '../../components/shared/DataTable';
import { StatusBadge } from '../../components/shared/StatusBadge';
import { CURRENT_MONTH, CURRENT_YEAR, TODAY } from './reportConstants';
import { ReportActionButtons } from './ReportActionButtons';
import { ReportPeriodFilters } from './ReportPeriodFilters';

const CUSTOM = 3;

// Mirrors Domain/Enums/MonthlySummaryStatus.cs exactly, same as MonthlySummariesPage.
const STATUS_OPTIONS = [
  { value: 0, key: 'Draft' },
  { value: 1, key: 'Verified' },
  { value: 2, key: 'Paid' },
] as const;

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

export default function SalariesReportPage() {
  const { t } = useTranslation(['reports', 'common']);

  const [periodType, setPeriodType] = useState<number>(2);
  const [date, setDate] = useState(TODAY);
  const [year, setYear] = useState(CURRENT_YEAR);
  const [month, setMonth] = useState(CURRENT_MONTH);
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [status, setStatus] = useState('');
  const [employeeId, setEmployeeId] = useState('');

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
    status: status === '' ? undefined : Number(status),
    employeeId: employeeId || undefined,
  };

  const { data: report, isLoading, error } = useGetApiReportsSalariesQuery(periodArgs, { skip: !customReady });

  const columns: ColumnDef<SalariesReportRowDto, unknown>[] = [
    { accessorKey: 'employeeName', header: t('columns.employee') },
    {
      accessorKey: 'platformIdNumber',
      header: t('salaries.columns.platformIdNumber'),
      cell: (info) => info.getValue() ?? '—',
    },
    {
      id: 'period',
      header: t('salaries.columns.period'),
      cell: ({ row }) => `${row.original.month}/${row.original.year}`,
    },
    { accessorKey: 'totalCompletedOrders', header: t('salaries.columns.orders') },
    {
      accessorKey: 'totalSalary',
      header: t('salaries.columns.totalSalary'),
      cell: (info) => `SAR ${Number(info.getValue()).toFixed(2)}`,
    },
    {
      accessorKey: 'totalAdvances',
      header: t('salaries.columns.totalAdvances'),
      cell: (info) => `SAR ${Number(info.getValue()).toFixed(2)}`,
    },
    {
      accessorKey: 'totalFines',
      header: t('salaries.columns.totalFines'),
      cell: (info) => `SAR ${Number(info.getValue()).toFixed(2)}`,
    },
    {
      accessorKey: 'netSalaryPayable',
      header: t('salaries.columns.netPayable'),
      cell: (info) => `SAR ${Number(info.getValue()).toFixed(2)}`,
    },
    {
      accessorKey: 'status',
      header: t('columns.status'),
      cell: (info) => <StatusBadge domain="monthlySummaryStatus" status={info.getValue() as string} />,
    },
  ];

  return (
    <Box>
      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
        <Box>
          <Typography variant="h4" gutterBottom>
            {t('salaries.title')}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {t('salaries.subtitle')}
          </Typography>
        </Box>
        <ReportActionButtons
          disabled={!report}
          exportUrl={(format) => salariesReportExportUrl({ ...periodArgs, format })}
          fallbackFileName="salaries-report"
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
          label={t('columns.status')}
          value={status}
          onChange={(event) => setStatus(event.target.value)}
          sx={{ minWidth: 160 }}
        >
          <MenuItem value="">{t('salaries.allStatuses')}</MenuItem>
          {STATUS_OPTIONS.map((option) => (
            <MenuItem key={option.value} value={option.value}>
              {t(`common:status.monthlySummaryStatus.${option.key}`)}
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
            <SummaryTile label={t('salaries.summary.totalSalary')} value={`SAR ${Number(report.totalSalary).toFixed(2)}`} />
            <SummaryTile label={t('salaries.summary.totalAdvances')} value={`SAR ${Number(report.totalAdvances).toFixed(2)}`} />
            <SummaryTile label={t('salaries.summary.totalFines')} value={`SAR ${Number(report.totalFines).toFixed(2)}`} />
            <SummaryTile label={t('salaries.summary.totalNetPayable')} value={`SAR ${Number(report.totalNetPayable).toFixed(2)}`} />
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
