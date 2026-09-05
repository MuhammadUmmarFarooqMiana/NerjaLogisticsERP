import { useState } from 'react';
import { Box, Button, Chip, FormControlLabel, Stack, Switch, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import { useGetApiInventoryItemsQuery } from '../../api/inventoryApi';
import type { InventoryItemDto } from '../../api/generated/apiSlice';
import { DataTable } from '../../components/shared/DataTable';
import { getPaginationMeta } from '../../lib/pagination';
import { CreateItemDialog } from './CreateItemDialog';
import { StockInDialog } from './StockInDialog';
import { StockOutDialog } from './StockOutDialog';
import { ItemLedgerDialog } from './ItemLedgerDialog';

export default function InventoryPage() {
  const { t } = useTranslation('inventory');
  const [lowStockOnly, setLowStockOnly] = useState(false);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const { data, isLoading, error } = useGetApiInventoryItemsQuery({ lowStockOnly, pageNumber: page + 1, pageSize });
  const rowCount = getPaginationMeta(data)?.totalCount ?? 0;

  const [createOpen, setCreateOpen] = useState(false);
  const [stockInTarget, setStockInTarget] = useState<InventoryItemDto | null>(null);
  const [stockOutTarget, setStockOutTarget] = useState<InventoryItemDto | null>(null);
  const [ledgerTarget, setLedgerTarget] = useState<InventoryItemDto | null>(null);

  const columns: ColumnDef<InventoryItemDto, unknown>[] = [
    { accessorKey: 'itemName', header: t('columns.itemName') },
    { accessorKey: 'unit', header: t('columns.unit') },
    { accessorKey: 'currentStock', header: t('columns.currentStock') },
    { accessorKey: 'reorderLevel', header: t('columns.reorderLevel') },
    {
      accessorKey: 'isLowStock',
      header: t('columns.status'),
      cell: (info) =>
        info.getValue() ? (
          <Chip size="small" color="warning" label={t('lowStock')} />
        ) : (
          <Chip size="small" color="success" label={t('inStock')} />
        ),
    },
    {
      id: 'actions',
      header: '',
      cell: ({ row }) => (
        <Stack direction="row" spacing={1}>
          <Button size="small" onClick={() => setStockInTarget(row.original)}>
            {t('stockIn')}
          </Button>
          <Button size="small" onClick={() => setStockOutTarget(row.original)}>
            {t('stockOut')}
          </Button>
          <Button size="small" onClick={() => setLedgerTarget(row.original)}>
            {t('viewLedger')}
          </Button>
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
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setCreateOpen(true)}>
          {t('addItem')}
        </Button>
      </Stack>

      <Stack direction="row" sx={{ my: 2 }}>
        <FormControlLabel
          control={
            <Switch
              checked={lowStockOnly}
              onChange={(event) => {
                setLowStockOnly(event.target.checked);
                setPage(0);
              }}
            />
          }
          label={t('filters.lowStockOnly')}
        />
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

      <CreateItemDialog open={createOpen} onClose={() => setCreateOpen(false)} />
      <StockInDialog open={!!stockInTarget} item={stockInTarget} onClose={() => setStockInTarget(null)} />
      <StockOutDialog open={!!stockOutTarget} item={stockOutTarget} onClose={() => setStockOutTarget(null)} />
      <ItemLedgerDialog open={!!ledgerTarget} item={ledgerTarget} onClose={() => setLedgerTarget(null)} />
    </Box>
  );
}
