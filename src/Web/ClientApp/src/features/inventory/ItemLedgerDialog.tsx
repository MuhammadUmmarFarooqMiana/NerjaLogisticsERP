import { Button, Dialog, DialogActions, DialogContent, DialogTitle } from '@mui/material';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import { useState } from 'react';
import { useGetApiInventoryItemsByItemIdHistoryQuery } from '../../api/inventoryApi';
import type { InventoryItemDto, StockMovementDto } from '../../api/generated/apiSlice';
import { DataTable } from '../../components/shared/DataTable';
import { formatDate } from '../../lib/formatDate';
import { getPaginationMeta } from '../../lib/pagination';

interface ItemLedgerDialogProps {
  open: boolean;
  item: InventoryItemDto | null;
  onClose: () => void;
}

export function ItemLedgerDialog({ open, item, onClose }: ItemLedgerDialogProps) {
  const { t, i18n } = useTranslation('inventory');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const { data, isLoading, error } = useGetApiInventoryItemsByItemIdHistoryQuery(
    { itemId: item?.id ?? '', pageNumber: page + 1, pageSize },
    { skip: !open || !item?.id }
  );
  const rowCount = getPaginationMeta(data)?.totalCount ?? 0;

  const columns: ColumnDef<StockMovementDto, unknown>[] = [
    {
      accessorKey: 'date',
      header: t('ledgerDialog.columns.date'),
      cell: (info) => formatDate(info.getValue() as string, i18n.language),
    },
    {
      accessorKey: 'type',
      header: t('ledgerDialog.columns.type'),
      cell: (info) => t(`ledgerDialog.type.${info.getValue() as string}`),
    },
    { accessorKey: 'quantity', header: t('ledgerDialog.columns.quantity') },
    {
      accessorKey: 'detail',
      header: t('ledgerDialog.columns.detail'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
  ];

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>{t('ledgerDialog.title', { itemName: item?.itemName })}</DialogTitle>
      <DialogContent>
        <DataTable
          columns={columns}
          data={data ?? []}
          isLoading={isLoading}
          error={error}
          emptyMessage={t('ledgerDialog.empty')}
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
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>{t('common:actions.cancel')}</Button>
      </DialogActions>
    </Dialog>
  );
}
