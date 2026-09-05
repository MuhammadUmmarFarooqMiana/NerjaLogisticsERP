import { useState } from 'react';
import { Box, Button, MenuItem, Stack, TextField, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import { useGetApiExpensesQuery } from '../../api/expensesApi';
import { useGetApiPlatformsQuery } from '../../api/generated/apiSlice';
import type { ExpenseDto } from '../../api/generated/apiSlice';
import { AppDatePicker } from '../../components/shared/AppDatePicker';
import { DataTable } from '../../components/shared/DataTable';
import { formatDate } from '../../lib/formatDate';
import { getPaginationMeta } from '../../lib/pagination';
import { CreateExpenseDialog } from './CreateExpenseDialog';
import { EXPENSE_CATEGORIES } from './expenseCategories';

export default function ExpensesPage() {
  const { t, i18n } = useTranslation('expenses');
  const { data: platforms } = useGetApiPlatformsQuery();
  const [category, setCategory] = useState('');
  const [platformId, setPlatformId] = useState('');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);

  const { data, isLoading, error } = useGetApiExpensesQuery({
    category: category === '' ? undefined : Number(category),
    platformId: platformId || undefined,
    startDate: startDate || undefined,
    endDate: endDate || undefined,
    pageNumber: page + 1,
    pageSize,
  });
  const rowCount = getPaginationMeta(data)?.totalCount ?? 0;

  const columns: ColumnDef<ExpenseDto, unknown>[] = [
    {
      accessorKey: 'category',
      header: t('columns.category'),
      cell: (info) => t(`categories.${info.getValue() as string}`),
    },
    {
      accessorKey: 'amount',
      header: t('columns.amount'),
      cell: (info) => `SAR ${Number(info.getValue()).toFixed(2)}`,
    },
    {
      accessorKey: 'expenseDate',
      header: t('columns.expenseDate'),
      cell: (info) => formatDate(info.getValue() as string, i18n.language),
    },
    {
      accessorKey: 'platformName',
      header: t('columns.platform'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
    {
      accessorKey: 'description',
      header: t('columns.description'),
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
          {t('addExpense')}
        </Button>
      </Stack>

      <Stack direction="row" spacing={2} sx={{ my: 2, flexWrap: 'wrap' }}>
        <TextField
          select
          size="small"
          label={t('filters.category')}
          value={category}
          onChange={(event) => {
            setCategory(event.target.value);
            setPage(0);
          }}
          sx={{ minWidth: 200 }}
        >
          <MenuItem value="">{t('filters.allCategories')}</MenuItem>
          {EXPENSE_CATEGORIES.map((option) => (
            <MenuItem key={option.value} value={option.value}>
              {t(`categories.${option.key}`)}
            </MenuItem>
          ))}
        </TextField>
        <TextField
          select
          size="small"
          label={t('filters.platform')}
          value={platformId}
          onChange={(event) => {
            setPlatformId(event.target.value);
            setPage(0);
          }}
          sx={{ minWidth: 200 }}
        >
          <MenuItem value="">{t('filters.allPlatforms')}</MenuItem>
          {(platforms ?? []).map((platform) => (
            <MenuItem key={platform.id} value={platform.id}>
              {platform.name}
            </MenuItem>
          ))}
        </TextField>
        <AppDatePicker
          label={t('filters.startDate')}
          value={startDate}
          onChange={(value) => {
            setStartDate(value);
            setPage(0);
          }}
          maxDate={endDate || undefined}
          slotProps={{ textField: { size: 'small' } }}
        />
        <AppDatePicker
          label={t('filters.endDate')}
          value={endDate}
          onChange={(value) => {
            setEndDate(value);
            setPage(0);
          }}
          minDate={startDate || undefined}
          slotProps={{ textField: { size: 'small' } }}
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

      <CreateExpenseDialog open={dialogOpen} onClose={() => setDialogOpen(false)} />
    </Box>
  );
}
