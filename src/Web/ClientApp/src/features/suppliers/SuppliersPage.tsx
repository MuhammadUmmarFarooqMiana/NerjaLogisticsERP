import { useState } from 'react';
import { Box, Button, IconButton, Stack, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutlineOutlined';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import { useDeleteApiSuppliersByIdMutation, useGetApiSuppliersQuery } from '../../api/suppliersApi';
import type { SupplierDto } from '../../api/generated/apiSlice';
import { ConfirmDialog } from '../../components/shared/ConfirmDialog';
import { DataTable } from '../../components/shared/DataTable';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { getPaginationMeta } from '../../lib/pagination';
import { SupplierDialog } from './SupplierDialog';

export default function SuppliersPage() {
  const { t } = useTranslation('suppliers');
  const toast = useToast();
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const { data, isLoading, error } = useGetApiSuppliersQuery({ pageNumber: page + 1, pageSize });
  const rowCount = getPaginationMeta(data)?.totalCount ?? 0;
  const [deleteSupplier, { isLoading: deleting }] = useDeleteApiSuppliersByIdMutation();

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editTarget, setEditTarget] = useState<SupplierDto | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<SupplierDto | null>(null);

  const openCreate = () => {
    setEditTarget(null);
    setDialogOpen(true);
  };

  const openEdit = (supplier: SupplierDto) => {
    setEditTarget(supplier);
    setDialogOpen(true);
  };

  const handleDelete = async () => {
    if (!deleteTarget?.id) return;
    try {
      await deleteSupplier({ id: deleteTarget.id }).unwrap();
      toast.success(t('deleteSuccess'));
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    } finally {
      setDeleteTarget(null);
    }
  };

  const columns: ColumnDef<SupplierDto, unknown>[] = [
    { accessorKey: 'name', header: t('columns.name') },
    {
      accessorKey: 'email',
      header: t('columns.email'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
    {
      accessorKey: 'phone',
      header: t('columns.phone'),
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
          {t('addSupplier')}
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

      <SupplierDialog open={dialogOpen} supplier={editTarget} onClose={() => setDialogOpen(false)} />

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
