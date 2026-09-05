import { useState } from 'react';
import { Box, Button, MenuItem, Stack, TextField, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import {
  useGetApiLeaveRequestsMeQuery,
  useGetApiLeaveRequestsQuery,
  usePostApiLeaveRequestsByIdApproveMutation,
} from '../../api/leaveRequestsApi';
import { useGetApiEmployeesQuery } from '../../api/employeesApi';
import type { LeaveRequestDto } from '../../api/generated/apiSlice';
import { useAppSelector } from '../../app/hooks';
import { useToast } from '../../components/feedback/ToastContext';
import { DataTable } from '../../components/shared/DataTable';
import { StatusBadge } from '../../components/shared/StatusBadge';
import { getApiErrorMessages } from '../../lib/apiError';
import { employeeOptionLabel } from '../../lib/employeeDisplay';
import { formatDate } from '../../lib/formatDate';
import { getPaginationMeta } from '../../lib/pagination';
import { Roles } from '../../lib/roles';
import { selectCurrentUser } from '../auth/authSlice';
import { LEAVE_STATUSES } from './leaveStatus';
import { RejectLeaveRequestDialog } from './RejectLeaveRequestDialog';
import { SubmitLeaveRequestDialog } from './SubmitLeaveRequestDialog';

function MyLeaveRequestsView() {
  const { t, i18n } = useTranslation('leaveRequests');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const { data, isLoading, error } = useGetApiLeaveRequestsMeQuery({ pageNumber: page + 1, pageSize });
  const rowCount = getPaginationMeta(data)?.totalCount ?? 0;
  const [dialogOpen, setDialogOpen] = useState(false);

  const columns: ColumnDef<LeaveRequestDto, unknown>[] = [
    {
      accessorKey: 'startDate',
      header: t('columns.startDate'),
      cell: (info) => formatDate(info.getValue() as string, i18n.language),
    },
    {
      accessorKey: 'endDate',
      header: t('columns.endDate'),
      cell: (info) => formatDate(info.getValue() as string, i18n.language),
    },
    {
      accessorKey: 'reason',
      header: t('columns.reason'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
    {
      accessorKey: 'status',
      header: t('columns.status'),
      cell: (info) => <StatusBadge domain="leaveStatus" status={info.getValue() as string} />,
    },
    {
      accessorKey: 'rejectionReason',
      header: t('columns.rejectionReason'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
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
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setDialogOpen(true)}>
          {t('submitLeave')}
        </Button>
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

      <SubmitLeaveRequestDialog open={dialogOpen} onClose={() => setDialogOpen(false)} />
    </Box>
  );
}

function LeaveRequestsReviewQueue() {
  const { t, i18n } = useTranslation('leaveRequests');
  const toast = useToast();
  const { data: employees } = useGetApiEmployeesQuery({});
  const [status, setStatus] = useState('');
  const [employeeId, setEmployeeId] = useState('');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const { data, isLoading, error } = useGetApiLeaveRequestsQuery({
    status: status === '' ? undefined : Number(status),
    employeeId: employeeId || undefined,
    pageNumber: page + 1,
    pageSize,
  });
  const rowCount = getPaginationMeta(data)?.totalCount ?? 0;
  const [approve] = usePostApiLeaveRequestsByIdApproveMutation();
  const [rejectTarget, setRejectTarget] = useState<string | null>(null);

  const handleApprove = async (id: string) => {
    try {
      await approve({ id }).unwrap();
      toast.success(t('approveSuccess'));
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  const columns: ColumnDef<LeaveRequestDto, unknown>[] = [
    { accessorKey: 'employeeName', header: t('columns.employee') },
    {
      accessorKey: 'startDate',
      header: t('columns.startDate'),
      cell: (info) => formatDate(info.getValue() as string, i18n.language),
    },
    {
      accessorKey: 'endDate',
      header: t('columns.endDate'),
      cell: (info) => formatDate(info.getValue() as string, i18n.language),
    },
    {
      accessorKey: 'reason',
      header: t('columns.reason'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
    {
      accessorKey: 'status',
      header: t('columns.status'),
      cell: (info) => <StatusBadge domain="leaveStatus" status={info.getValue() as string} />,
    },
    {
      accessorKey: 'reviewedByName',
      header: t('columns.reviewedBy'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
    {
      id: 'actions',
      header: '',
      cell: ({ row }) =>
        row.original.status === 'Pending' ? (
          <Stack direction="row" spacing={1}>
            <Button size="small" onClick={() => void handleApprove(row.original.id!)}>
              {t('approve')}
            </Button>
            <Button size="small" color="error" onClick={() => setRejectTarget(row.original.id!)}>
              {t('reject')}
            </Button>
          </Stack>
        ) : null,
    },
  ];

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('reviewQueue.title')}
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        {t('reviewQueue.subtitle')}
      </Typography>

      <Stack direction="row" spacing={2} sx={{ mb: 2, flexWrap: 'wrap' }}>
        <TextField
          select
          size="small"
          label={t('reviewQueue.employee')}
          value={employeeId}
          onChange={(event) => {
            setEmployeeId(event.target.value);
            setPage(0);
          }}
          sx={{ minWidth: 220 }}
        >
          <MenuItem value="">{t('reviewQueue.allEmployees')}</MenuItem>
          {(employees ?? []).map((employee) => (
            <MenuItem key={employee.id} value={employee.id}>
              {employeeOptionLabel(employee)}
            </MenuItem>
          ))}
        </TextField>
        <TextField
          select
          size="small"
          label={t('columns.status')}
          value={status}
          onChange={(event) => {
            setStatus(event.target.value);
            setPage(0);
          }}
          sx={{ minWidth: 200 }}
        >
          <MenuItem value="">{t('reviewQueue.allStatuses')}</MenuItem>
          {LEAVE_STATUSES.map((option) => (
            <MenuItem key={option.value} value={option.value}>
              {t(`common:status.leaveStatus.${option.key}`)}
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

      <RejectLeaveRequestDialog
        open={!!rejectTarget}
        leaveRequestId={rejectTarget}
        onClose={() => setRejectTarget(null)}
      />
    </Box>
  );
}

export default function LeaveRequestsPage() {
  const user = useAppSelector(selectCurrentUser);
  const isReviewer = !!user?.roles.some((role) => role === Roles.Administrator || role === Roles.Supervisor);

  return isReviewer ? <LeaveRequestsReviewQueue /> : <MyLeaveRequestsView />;
}
