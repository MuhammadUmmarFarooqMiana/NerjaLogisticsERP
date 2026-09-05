import { useEffect, useState } from 'react';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Alert, Box, Button, CircularProgress, Paper, Stack, TextField, Typography } from '@mui/material';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import {
  useGetApiDailyOrdersMeQuery,
  useGetApiDailyOrdersQuery,
  usePostApiDailyOrdersByIdApproveMutation,
  usePostApiDailyOrdersCloseMutation,
  usePostApiDailyOrdersUpsertOrderCountMutation,
} from '../../api/dailyOrdersApi';
import type { DailyOrderListItemDto } from '../../api/generated/apiSlice';
import { useAppSelector } from '../../app/hooks';
import { useToast } from '../../components/feedback/ToastContext';
import { AppDatePicker } from '../../components/shared/AppDatePicker';
import { ConfirmDialog } from '../../components/shared/ConfirmDialog';
import { DataTable } from '../../components/shared/DataTable';
import { StatusBadge } from '../../components/shared/StatusBadge';
import { getApiErrorMessages } from '../../lib/apiError';
import { formatDateTime } from '../../lib/formatDate';
import { getPaginationMeta } from '../../lib/pagination';
import { Roles } from '../../lib/roles';
import { selectCurrentUser } from '../auth/authSlice';
import { CorrectDailyOrderDialog } from './CorrectDailyOrderDialog';
import { OrderHistorySection } from './OrderHistorySection';
import { RejectDailyOrderDialog } from './RejectDailyOrderDialog';
import { buildDailyOrderSchema, type DailyOrderFormValues } from './schemas';

function toIsoDate(date: Date): string {
  return date.toISOString().slice(0, 10);
}

function MyDailyOrderCard() {
  const { t, i18n } = useTranslation('dailyOrders');
  const toast = useToast();
  const { data: order, isLoading } = useGetApiDailyOrdersMeQuery();
  const [upsert, { isLoading: saving }] = usePostApiDailyOrdersUpsertOrderCountMutation();
  const [close, { isLoading: closing }] = usePostApiDailyOrdersCloseMutation();
  const [closeDialogOpen, setCloseDialogOpen] = useState(false);

  const isClosed = order?.status === 'Closed';
  // Only an Open (or not-yet-started) day is still editable by the rider —
  // Closed is awaiting a supervisor's review, and Approved/Rejected are
  // reviewer-owned terminal states from here on.
  const canEdit = !order || order.status === 'Open';
  const isRejected = order?.status === 'Rejected';

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<DailyOrderFormValues>({
    resolver: zodResolver(buildDailyOrderSchema(t)),
    defaultValues: { completedOrders: Number(order?.completedOrders ?? 0) },
  });

  // Resync the field once the query resolves — defaultValues only apply at first mount,
  // and the order isn't loaded yet on that first render.
  useEffect(() => {
    if (order) reset({ completedOrders: Number(order.completedOrders) });
  }, [order, reset]);

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  const onSubmit = async (values: DailyOrderFormValues) => {
    try {
      await upsert({ upsertDailyOrderCommand: { completedOrders: values.completedOrders } }).unwrap();
      toast.success(t('saveSuccess'));
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  const handleClose = async () => {
    try {
      await close().unwrap();
      toast.success(t('closeSuccess'));
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    } finally {
      setCloseDialogOpen(false);
    }
  };

  return (
    <Paper variant="outlined" sx={{ p: 3, maxWidth: 480 }}>
      <Stack spacing={2}>
        {order && (
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
            <StatusBadge domain="dailyOrderStatus" status={order.status ?? 'Open'} />
            {order.closedAt && (
              <Typography variant="caption" color="text.secondary">
                {formatDateTime(order.closedAt, i18n.language)}
              </Typography>
            )}
          </Stack>
        )}

        {!order && (
          <Typography variant="body2" color="text.secondary">
            {t('noEntryYet')}
          </Typography>
        )}

        {isClosed && <Alert severity="info">{t('closedNotice')}</Alert>}
        {isRejected && (
          <Alert severity="error">{t('rejectedNotice', { reason: order?.reviewNote ?? '—' })}</Alert>
        )}

        <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
          <TextField
            {...register('completedOrders', { valueAsNumber: true })}
            label={t('completedOrders')}
            type="number"
            required
            fullWidth
            disabled={!canEdit}
            slotProps={{ htmlInput: { min: 0 } }}
            error={!!errors.completedOrders}
            helperText={errors.completedOrders?.message}
          />
          <Stack direction="row" spacing={2} sx={{ mt: 2 }}>
            <Button
              type="submit"
              variant="contained"
              disabled={!canEdit || saving}
              startIcon={saving ? <CircularProgress size={18} color="inherit" /> : undefined}
            >
              {t('save')}
            </Button>
            <Button
              variant="outlined"
              color="error"
              disabled={!order || !canEdit}
              onClick={() => setCloseDialogOpen(true)}
            >
              {t('closeDay')}
            </Button>
          </Stack>
        </Box>
      </Stack>

      <ConfirmDialog
        open={closeDialogOpen}
        title={t('closeConfirmTitle')}
        description={t('closeConfirmDescription')}
        destructive
        loading={closing}
        onConfirm={() => void handleClose()}
        onCancel={() => setCloseDialogOpen(false)}
      />
    </Paper>
  );
}

function TeamDailyOrdersTable() {
  const { t, i18n } = useTranslation('dailyOrders');
  const toast = useToast();
  const [date, setDate] = useState(() => toIsoDate(new Date()));
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const { data, isLoading, error } = useGetApiDailyOrdersQuery({ date, pageNumber: page + 1, pageSize });
  const rowCount = getPaginationMeta(data)?.totalCount ?? 0;

  const [approve, { isLoading: approving }] = usePostApiDailyOrdersByIdApproveMutation();
  const [rejectTarget, setRejectTarget] = useState<string | null>(null);
  const [correctTarget, setCorrectTarget] = useState<DailyOrderListItemDto | null>(null);

  const handleApprove = async (id: string) => {
    try {
      await approve({ id }).unwrap();
      toast.success(t('team.approveSuccess'));
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  const columns: ColumnDef<DailyOrderListItemDto, unknown>[] = [
    { accessorKey: 'employeeName', header: t('team.columns.name') },
    { accessorKey: 'completedOrders', header: t('team.columns.completedOrders') },
    {
      accessorKey: 'status',
      header: t('team.columns.status'),
      cell: (info) => <StatusBadge domain="dailyOrderStatus" status={info.getValue() as string} />,
    },
    {
      accessorKey: 'closedAt',
      header: t('team.columns.closedAt'),
      cell: (info) => formatDateTime(info.getValue() as string | null, i18n.language) || '—',
    },
    {
      accessorKey: 'reviewNote',
      header: t('team.columns.reviewNote'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
    {
      id: 'actions',
      header: '',
      cell: ({ row }) => {
        if (row.original.status === 'Closed') {
          return (
            <Stack direction="row" spacing={1}>
              <Button size="small" disabled={approving} onClick={() => void handleApprove(row.original.id!)}>
                {t('team.approve')}
              </Button>
              <Button size="small" color="error" onClick={() => setRejectTarget(row.original.id!)}>
                {t('team.reject')}
              </Button>
            </Stack>
          );
        }
        if (row.original.status === 'Rejected') {
          return (
            <Button size="small" onClick={() => setCorrectTarget(row.original)}>
              {t('team.correct')}
            </Button>
          );
        }
        return null;
      },
    },
  ];

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('team.title')}
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        {t('team.subtitle')}
      </Typography>

      <AppDatePicker
        label={t('team.dateLabel')}
        value={date}
        onChange={(value) => {
          setDate(value);
          setPage(0);
        }}
        slotProps={{ textField: { size: 'small', sx: { mb: 2 } } }}
      />

      <DataTable
        columns={columns}
        data={data ?? []}
        isLoading={isLoading}
        error={error}
        emptyMessage={t('team.empty')}
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

      <RejectDailyOrderDialog open={!!rejectTarget} dailyOrderId={rejectTarget} onClose={() => setRejectTarget(null)} />
      <CorrectDailyOrderDialog open={!!correctTarget} order={correctTarget} onClose={() => setCorrectTarget(null)} />
    </Box>
  );
}

export default function DailyOrdersPage() {
  const { t } = useTranslation('dailyOrders');
  const user = useAppSelector(selectCurrentUser);
  const isFleetStaff = !!user?.roles.some((role) => role === Roles.Administrator || role === Roles.Supervisor);

  if (isFleetStaff) {
    return (
      <Box>
        <TeamDailyOrdersTable />
        <OrderHistorySection />
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('title')}
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        {t('subtitle')}
      </Typography>
      <MyDailyOrderCard />
      <OrderHistorySection />
    </Box>
  );
}
