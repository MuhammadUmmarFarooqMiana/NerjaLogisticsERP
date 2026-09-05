import { useState } from 'react';
import { Alert, Box, Grid, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import { useGetApiEmployeesQuery } from '../../api/employeesApi';
import type { LeavesReportRowDto } from '../../api/generated/apiSlice';
import { leavesReportExportUrl, useGetApiReportsLeavesQuery } from '../../api/reportsApi';
import { DataTable } from '../../components/shared/DataTable';
import { StatusBadge } from '../../components/shared/StatusBadge';
import { formatDate } from '../../lib/formatDate';
import { LEAVE_STATUSES } from '../leave-requests/leaveStatus';
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

export default function LeavesReportPage() {
  const { t, i18n } = useTranslation(['reports', 'common']);

  const [periodType, setPeriodType] = useState<number>(0);
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

  const { data: report, isLoading, error } = useGetApiReportsLeavesQuery(periodArgs, { skip: !customReady });

  const columns: ColumnDef<LeavesReportRowDto, unknown>[] = [
    { accessorKey: 'employeeName', header: t('columns.employee') },
    {
      accessorKey: 'startDate',
      header: t('leaves.columns.startDate'),
      cell: (info) => formatDate(info.getValue() as string, i18n.language),
    },
    {
      accessorKey: 'endDate',
      header: t('leaves.columns.endDate'),
      cell: (info) => formatDate(info.getValue() as string, i18n.language),
    },
    { accessorKey: 'days', header: t('leaves.columns.days') },
    {
      accessorKey: 'status',
      header: t('columns.status'),
      cell: (info) => <StatusBadge domain="leaveStatus" status={info.getValue() as string} />,
    },
    {
      accessorKey: 'reviewedByName',
      header: t('leaves.columns.reviewedBy'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
  ];

  return (
    <Box>
      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
        <Box>
          <Typography variant="h4" gutterBottom>
            {t('leaves.title')}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {t('leaves.subtitle')}
          </Typography>
        </Box>
        <ReportActionButtons
          disabled={!report}
          exportUrl={(format) => leavesReportExportUrl({ ...periodArgs, format })}
          fallbackFileName="leaves-report"
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
          <MenuItem value="">{t('leaves.allStatuses')}</MenuItem>
          {LEAVE_STATUSES.map((option) => (
            <MenuItem key={option.value} value={option.value}>
              {t(`common:status.leaveStatus.${option.key}`)}
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
            <SummaryTile label={t('leaves.summary.totalRequests')} value={String(report.totalRequests)} />
            <SummaryTile label={t('leaves.summary.totalDays')} value={String(report.totalDays)} />
            <SummaryTile
              label={t('leaves.summary.approvedRejectedPending')}
              value={`${report.approvedCount} / ${report.rejectedCount} / ${report.pendingCount}`}
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
