import { useState } from 'react';
import { Box, Button, MenuItem, Stack, TextField, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import {
  useGetApiMonthlySummariesMeQuery,
  useGetApiMonthlySummariesQuery,
  usePostApiMonthlySummariesByIdMarkAsPaidMutation,
  usePostApiMonthlySummariesByIdVerifyMutation,
} from '../../api/monthlySummariesApi';
import { useGetApiEmployeesQuery } from '../../api/employeesApi';
import type { MonthlySummaryDto } from '../../api/generated/apiSlice';
import { useAppSelector } from '../../app/hooks';
import { useToast } from '../../components/feedback/ToastContext';
import { DataTable } from '../../components/shared/DataTable';
import { StatusBadge } from '../../components/shared/StatusBadge';
import { getApiErrorMessages } from '../../lib/apiError';
import { employeeOptionLabel } from '../../lib/employeeDisplay';
import { getPaginationMeta } from '../../lib/pagination';
import { Roles } from '../../lib/roles';
import { selectCurrentUser } from '../auth/authSlice';
import { GenerateSummaryDialog } from './GenerateSummaryDialog';
import { MarkAsPaidDialog } from './MarkAsPaidDialog';

const STATUS_OPTIONS = [
  { value: 0, key: 'Draft' },
  { value: 1, key: 'Verified' },
  { value: 2, key: 'Paid' },
] as const;

// This theme deliberately remaps palette.error.main to the brand orange (see theme.ts),
// so a literal red is used for deduction amounts instead — 'error.main' wouldn't render as red.
const DEDUCTION_COLOR = '#D32F2F';

function MyMonthlySummariesView() {
  const { t } = useTranslation('monthlySummaries');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const { data, isLoading, error } = useGetApiMonthlySummariesMeQuery({ pageNumber: page + 1, pageSize });
  const rowCount = getPaginationMeta(data)?.totalCount ?? 0;

  const columns: ColumnDef<MonthlySummaryDto, unknown>[] = [
    {
      id: 'period',
      header: t('columns.period'),
      cell: ({ row }) => `${row.original.month}/${row.original.year}`,
    },
    { accessorKey: 'totalCompletedOrders', header: t('columns.orders') },
    {
      accessorKey: 'totalSalary',
      header: t('columns.totalSalary'),
      cell: (info) => (
        <Box component="span" sx={{ color: 'success.main', fontWeight: 600 }}>
          SAR {Number(info.getValue()).toFixed(2)}
        </Box>
      ),
    },
    {
      accessorKey: 'totalAdvances',
      header: t('columns.totalAdvances'),
      cell: (info) => (
        <Box component="span" sx={{ color: DEDUCTION_COLOR, fontWeight: 600 }}>
          SAR {Number(info.getValue()).toFixed(2)}
        </Box>
      ),
    },
    {
      accessorKey: 'totalFines',
      header: t('columns.totalFines'),
      cell: (info) => (
        <Box component="span" sx={{ color: DEDUCTION_COLOR, fontWeight: 600 }}>
          SAR {Number(info.getValue()).toFixed(2)}
        </Box>
      ),
    },
    {
      accessorKey: 'netSalaryPayable',
      header: t('columns.netPayable'),
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
      <Typography variant="h4" gutterBottom>
        {t('myTitle')}
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        {t('mySubtitle')}
      </Typography>

      <DataTable
        columns={columns}
        data={data ?? []}
        isLoading={isLoading}
        error={error}
        emptyMessage={t('empty')}
        serverPagination={{
          pageIndex: page,
          pageSize,
          rowCount,
          onPageChange: setPage,
          onPageSizeChange: (size) => {
            setPageSize(size);
            setPage(0);
          },
        }}
      />
    </Box>
  );
}

function MonthlySummariesManagementView() {
  const { t } = useTranslation('monthlySummaries');
  const toast = useToast();
  const user = useAppSelector(selectCurrentUser);
  const isAccountant = !!user?.roles.some((role) => role === Roles.Accountant);

  const { data: employees } = useGetApiEmployeesQuery({});
  const [employeeId, setEmployeeId] = useState('');
  const [year, setYear] = useState('');
  const [month, setMonth] = useState('');
  const [status, setStatus] = useState('');
  const [generateOpen, setGenerateOpen] = useState(false);
  const [markAsPaidTarget, setMarkAsPaidTarget] = useState<MonthlySummaryDto | null>(null);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);

  const { data, isLoading, error } = useGetApiMonthlySummariesQuery({
    employeeId: employeeId || undefined,
    year: year ? Number(year) : undefined,
    month: month ? Number(month) : undefined,
    status: status === '' ? undefined : Number(status),
    pageNumber: page + 1,
    pageSize,
  });
  const rowCount = getPaginationMeta(data)?.totalCount ?? 0;

  const [verify, { isLoading: verifying }] = usePostApiMonthlySummariesByIdVerifyMutation();
  const [markAsPaid, { isLoading: paying }] = usePostApiMonthlySummariesByIdMarkAsPaidMutation();

  const handleVerify = async (id: string) => {
    try {
      await verify({ id }).unwrap();
      toast.success(t('verifySuccess'));
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  const handleMarkAsPaid = async (paymentReference: string) => {
    if (!markAsPaidTarget?.id) return;
    try {
      await markAsPaid({ id: markAsPaidTarget.id, body: paymentReference || null }).unwrap();
      toast.success(t('markAsPaidSuccess'));
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    } finally {
      setMarkAsPaidTarget(null);
    }
  };

  const columns: ColumnDef<MonthlySummaryDto, unknown>[] = [
    { accessorKey: 'employeeName', header: t('columns.employee') },
    {
      id: 'period',
      header: t('columns.period'),
      cell: ({ row }) => `${row.original.month}/${row.original.year}`,
    },
    { accessorKey: 'totalCompletedOrders', header: t('columns.orders') },
    {
      accessorKey: 'totalSalary',
      header: t('columns.totalSalary'),
      cell: (info) => (
        <Box component="span" sx={{ color: 'success.main', fontWeight: 600 }}>
          SAR {Number(info.getValue()).toFixed(2)}
        </Box>
      ),
    },
    {
      accessorKey: 'totalAdvances',
      header: t('columns.totalAdvances'),
      cell: (info) => (
        <Box component="span" sx={{ color: DEDUCTION_COLOR, fontWeight: 600 }}>
          SAR {Number(info.getValue()).toFixed(2)}
        </Box>
      ),
    },
    {
      accessorKey: 'totalFines',
      header: t('columns.totalFines'),
      cell: (info) => (
        <Box component="span" sx={{ color: DEDUCTION_COLOR, fontWeight: 600 }}>
          SAR {Number(info.getValue()).toFixed(2)}
        </Box>
      ),
    },
    {
      accessorKey: 'netSalaryPayable',
      header: t('columns.netPayable'),
      cell: (info) => `SAR ${Number(info.getValue()).toFixed(2)}`,
    },
    {
      accessorKey: 'status',
      header: t('columns.status'),
      cell: (info) => <StatusBadge domain="monthlySummaryStatus" status={info.getValue() as string} />,
    },
    ...(isAccountant
      ? [
          {
            id: 'actions',
            header: '',
            cell: ({ row }: { row: { original: MonthlySummaryDto } }) => {
              if (row.original.status === 'Draft') {
                return (
                  <Button size="small" onClick={() => void handleVerify(row.original.id!)} disabled={verifying}>
                    {t('verify')}
                  </Button>
                );
              }
              if (row.original.status === 'Verified') {
                return (
                  <Button size="small" onClick={() => setMarkAsPaidTarget(row.original)}>
                    {t('markAsPaid')}
                  </Button>
                );
              }
              return null;
            },
          } satisfies ColumnDef<MonthlySummaryDto, unknown>,
        ]
      : []),
  ];

  return (
    <Box>
      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
        <Box>
          <Typography variant="h4" gutterBottom>
            {t('title')}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {t('subtitle')}
          </Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setGenerateOpen(true)}>
          {t('generate')}
        </Button>
      </Stack>

      <Stack direction="row" spacing={2} sx={{ my: 2, flexWrap: 'wrap' }}>
        <TextField
          select
          size="small"
          label={t('filters.employee')}
          value={employeeId}
          onChange={(event) => {
            setEmployeeId(event.target.value);
            setPage(0);
          }}
          sx={{ minWidth: 200 }}
        >
          <MenuItem value="">{t('filters.allEmployees')}</MenuItem>
          {(employees ?? []).map((employee) => (
            <MenuItem key={employee.id} value={employee.id}>
              {employeeOptionLabel(employee)}
            </MenuItem>
          ))}
        </TextField>
        <TextField
          label={t('filters.year')}
          type="number"
          size="small"
          value={year}
          onChange={(event) => {
            setYear(event.target.value);
            setPage(0);
          }}
          sx={{ width: 120 }}
        />
        <TextField
          label={t('filters.month')}
          type="number"
          size="small"
          value={month}
          onChange={(event) => {
            setMonth(event.target.value);
            setPage(0);
          }}
          slotProps={{ htmlInput: { min: 1, max: 12 } }}
          sx={{ width: 100 }}
        />
        <TextField
          select
          size="small"
          label={t('filters.status')}
          value={status}
          onChange={(event) => {
            setStatus(event.target.value);
            setPage(0);
          }}
          sx={{ minWidth: 160 }}
        >
          <MenuItem value="">{t('filters.allStatuses')}</MenuItem>
          {STATUS_OPTIONS.map((option) => (
            <MenuItem key={option.value} value={option.value}>
              {t(`common:status.monthlySummaryStatus.${option.key}`)}
            </MenuItem>
          ))}
        </TextField>
      </Stack>

      <DataTable
        columns={columns}
        data={data ?? []}
        isLoading={isLoading}
        error={error}
        emptyMessage={t('empty')}
        serverPagination={{
          pageIndex: page,
          pageSize,
          rowCount,
          onPageChange: setPage,
          onPageSizeChange: (size) => {
            setPageSize(size);
            setPage(0);
          },
        }}
      />

      <GenerateSummaryDialog open={generateOpen} onClose={() => setGenerateOpen(false)} />

      <MarkAsPaidDialog
        open={!!markAsPaidTarget}
        loading={paying}
        onConfirm={(paymentReference) => void handleMarkAsPaid(paymentReference)}
        onCancel={() => setMarkAsPaidTarget(null)}
      />
    </Box>
  );
}

export default function MonthlySummariesPage() {
  const user = useAppSelector(selectCurrentUser);
  const isFinance = !!user?.roles.some((role) => role === Roles.Administrator || role === Roles.Accountant);

  return isFinance ? <MonthlySummariesManagementView /> : <MyMonthlySummariesView />;
}
