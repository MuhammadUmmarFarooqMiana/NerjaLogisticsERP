import { useState } from 'react';
import { Box, Button, IconButton, Stack, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutlineOutlined';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import { useDeleteApiMechanicsByIdMutation, useGetApiMechanicsQuery } from '../../api/mechanicsApi';
import type { MechanicDto } from '../../api/generated/apiSlice';
import { ConfirmDialog } from '../../components/shared/ConfirmDialog';
import { DataTable } from '../../components/shared/DataTable';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { getPaginationMeta } from '../../lib/pagination';
import { MechanicDialog } from './MechanicDialog';

export default function MechanicsPage() {
  const { t } = useTranslation('mechanics');
  const toast = useToast();
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const { data, isLoading, error } = useGetApiMechanicsQuery({ pageNumber: page + 1, pageSize });
  const rowCount = getPaginationMeta(data)?.totalCount ?? 0;
  const [deleteMechanic, { isLoading: deleting }] = useDeleteApiMechanicsByIdMutation();

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editTarget, setEditTarget] = useState<MechanicDto | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<MechanicDto | null>(null);

  const openCreate = () => {
    setEditTarget(null);
    setDialogOpen(true);
  };

  const openEdit = (mechanic: MechanicDto) => {
    setEditTarget(mechanic);
    setDialogOpen(true);
  };

  const handleDelete = async () => {
    if (!deleteTarget?.id) return;
    try {
      await deleteMechanic({ id: deleteTarget.id }).unwrap();
      toast.success(t('deleteSuccess'));
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    } finally {
      setDeleteTarget(null);
    }
  };

  const columns: ColumnDef<MechanicDto, unknown>[] = [
    { accessorKey: 'name', header: t('columns.name') },
    {
      accessorKey: 'phone',
      header: t('columns.phone'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
    {
      accessorKey: 'email',
      header: t('columns.email'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
    {
      accessorKey: 'specialty',
      header: t('columns.specialty'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
    {
      accessorKey: 'address',
      header: t('columns.address'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
    {
      id: 'actions',
      header: '',
      cell: ({ row }) => (
        <Stack direction="row" spacing={0.5}>
          <IconButton size="small" aria-label={t('edit')} onClick={() => openEdit(row.original)}>
            <EditOutlinedIcon fontSize="small" />
          </IconButton>
          <IconButton size="small" aria-label={t('delete')} onClick={() => setDeleteTarget(row.original)}>
            <DeleteOutlineIcon fontSize="small" />
          </IconButton>
        </Stack>
      ),
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
        <Button variant="contained" startIcon={<AddIcon />} onClick={openCreate}>
          {t('addMechanic')}
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

      <MechanicDialog open={dialogOpen} mechanic={editTarget} onClose={() => setDialogOpen(false)} />

      <ConfirmDialog
        open={!!deleteTarget}
        title={t('deleteConfirmTitle')}
        description={t('deleteConfirmDescription', { name: deleteTarget?.name })}
        destructive
        loading={deleting}
        onConfirm={() => void handleDelete()}
        onCancel={() => setDeleteTarget(null)}
      />
    </Box>
  );
}
