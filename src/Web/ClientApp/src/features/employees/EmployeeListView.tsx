import { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Box, Button, IconButton, MenuItem, Stack, TextField, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutlineOutlined';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import { DataTable } from '../../components/shared/DataTable';
import { StatusBadge } from '../../components/shared/StatusBadge';
import { useDeleteApiEmployeesByIdMutation, useGetApiEmployeesQuery } from '../../api/employeesApi';
import type { EmployeeListItemDto } from '../../api/generated/apiSlice';
import { useAppSelector } from '../../app/hooks';
import { ConfirmDialog } from '../../components/shared/ConfirmDialog';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { getPaginationMeta } from '../../lib/pagination';
import { Roles } from '../../lib/roles';
import { selectCurrentUser } from '../auth/authSlice';
import { CreateEmployeeDialog } from './CreateEmployeeDialog';

const STATUS_OPTIONS = ['Incomplete', 'PendingReview', 'Active', 'Suspended', 'Rejected', 'Terminated'];

interface EmployeeListViewProps {
  title: string;
  /** Locks the list to one status (Pending Approvals) — hides the status filter and admin actions. */
  fixedStatus?: string;
  emptyMessage?: string;
}

export function EmployeeListView({ title, fixedStatus, emptyMessage }: EmployeeListViewProps) {
  const { t } = useTranslation('employees');
  const navigate = useNavigate();
  const toast = useToast();
  const user = useAppSelector(selectCurrentUser);
  const isAdmin = !!user?.roles.some((role) => role === Roles.Administrator);
  const [status, setStatus] = useState('');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [createOpen, setCreateOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<EmployeeListItemDto | null>(null);

  const effectiveStatus = fixedStatus ?? (status || undefined);
  const { data, isLoading, error } = useGetApiEmployeesQuery({
    status: effectiveStatus,
    pageNumber: page + 1,
    pageSize,
  });
  const rowCount = getPaginationMeta(data)?.totalCount ?? 0;
  const [deleteEmployee, { isLoading: deleting }] = useDeleteApiEmployeesByIdMutation();

  const handleDelete = async () => {
    if (!deleteTarget?.id) return;
    try {
      await deleteEmployee({ id: deleteTarget.id }).unwrap();
      toast.success(t('admin.deleteSuccess'));
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('detail.genericError')).join('\n'));
    } finally {
      setDeleteTarget(null);
    }
  };

  const columns = useMemo<ColumnDef<EmployeeListItemDto, unknown>[]>(
    () => [
      { accessorKey: 'fullName', header: t('list.columns.name') },
      {
        accessorKey: 'platformName',
        header: t('list.columns.platform'),
        cell: (info) => (info.getValue() as string | null) ?? '—',
      },
      {
        accessorKey: 'accountStatus',
        header: t('list.columns.status'),
        cell: (info) => <StatusBadge domain="accountStatus" status={info.getValue() as string} />,
      },
      {
        accessorKey: 'iqamaNumber',
        header: t('list.columns.iqama'),
        cell: (info) => (info.getValue() as string | null) ?? '—',
      },
      {
        accessorKey: 'joiningDate',
        header: t('list.columns.joiningDate'),
        cell: (info) => (info.getValue() as string | null) ?? '—',
      },
      ...(isAdmin && !fixedStatus
        ? [
            {
              id: 'actions',
              header: '',
              cell: ({ row }: { row: { original: EmployeeListItemDto } }) => (
                <IconButton
                  size="small"
                  aria-label={t('admin.delete')}
                  onClick={(event) => {
                    event.stopPropagation();
                    setDeleteTarget(row.original);
                  }}
                >
                  <DeleteOutlineIcon fontSize="small" />
                </IconButton>
              ),
            } satisfies ColumnDef<EmployeeListItemDto, unknown>,
          ]
        : []),
    ],
    [t, isAdmin, fixedStatus]
  );

  return (
    <Box>
      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
        <Box>
          {title && (
            <Typography variant="h4" gutterBottom>
              {title}
            </Typography>
          )}
        </Box>
        {isAdmin && !fixedStatus && (
          <Button variant="contained" startIcon={<AddIcon />} onClick={() => setCreateOpen(true)}>
            {t('admin.addEmployee')}
          </Button>
        )}
      </Stack>
      {!fixedStatus && (
        <Stack direction="row" spacing={2} sx={{ mb: 2 }}>
          <TextField
            select
            size="small"
            label={t('list.filterStatus')}
            value={status}
            onChange={(event) => {
              setStatus(event.target.value);
              setPage(0);
            }}
            sx={{ minWidth: 220 }}
          >
            <MenuItem value="">{t('list.allStatuses')}</MenuItem>
            {STATUS_OPTIONS.map((option) => (
              <MenuItem key={option} value={option}>
                {t(`common:status.accountStatus.${option}`)}
              </MenuItem>
            ))}
          </TextField>
        </Stack>
      )}
      <DataTable
        columns={columns}
        data={data ?? []}
        isLoading={isLoading}
        error={error}
        emptyMessage={emptyMessage}
        onRowClick={(row) => navigate(`/employees/${row.id}`)}
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

      {isAdmin && !fixedStatus && (
        <>
          <CreateEmployeeDialog open={createOpen} onClose={() => setCreateOpen(false)} />
          <ConfirmDialog
            open={!!deleteTarget}
            title={t('admin.deleteConfirmTitle')}
            description={t('admin.deleteConfirmDescription', { name: deleteTarget?.fullName })}
            destructive
            loading={deleting}
            onConfirm={() => void handleDelete()}
            onCancel={() => setDeleteTarget(null)}
          />
        </>
      )}
    </Box>
  );
}
