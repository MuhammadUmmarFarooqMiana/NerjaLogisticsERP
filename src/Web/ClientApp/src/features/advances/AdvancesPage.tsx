import { useState } from 'react';
import { Box, Button, MenuItem, Stack, TextField, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import { useGetApiEmployeesQuery } from '../../api/employeesApi';
import { useGetApiAdvancesMeQuery, useGetApiAdvancesQuery } from '../../api/advancesApi';
import type { AdvanceDto } from '../../api/generated/apiSlice';
import { useAppSelector } from '../../app/hooks';
import { AppDatePicker } from '../../components/shared/AppDatePicker';
import { DataTable } from '../../components/shared/DataTable';
import { employeeOptionLabel } from '../../lib/employeeDisplay';
import { formatDate } from '../../lib/formatDate';
import { getPaginationMeta } from '../../lib/pagination';
import { Roles } from '../../lib/roles';
import { selectCurrentUser } from '../auth/authSlice';
import { CreateAdvanceDialog } from './CreateAdvanceDialog';

function MyAdvancesView() {
  const { t, i18n } = useTranslation('advances');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const { data, isLoading, error } = useGetApiAdvancesMeQuery({ pageNumber: page + 1, pageSize });
  const rowCount = getPaginationMeta(data)?.totalCount ?? 0;

  const columns: ColumnDef<AdvanceDto, unknown>[] = [
    {
      accessorKey: 'amount',
      header: t('columns.amount'),
      cell: (info) => `SAR ${Number(info.getValue()).toFixed(2)}`,
    },
    {
      accessorKey: 'advanceDate',
      header: t('columns.advanceDate'),
      cell: (info) => formatDate(info.getValue() as string, i18n.language),
    },
    {
      accessorKey: 'remarks',
      header: t('columns.remarks'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
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

function AdvancesManagementView() {
  const { t, i18n } = useTranslation('advances');
  const user = useAppSelector(selectCurrentUser);
  // CreateAdvanceCommand allows Administrator and Accountant on the backend — hide
  // the action for roles that would 403.
  const canCreate = !!user?.roles.some((role) => role === Roles.Administrator || role === Roles.Accountant);
  const { data: employees } = useGetApiEmployeesQuery({});
  const [employeeId, setEmployeeId] = useState('');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);

  const { data, isLoading, error } = useGetApiAdvancesQuery({
    employeeId: employeeId || undefined,
    startDate: startDate || undefined,
    endDate: endDate || undefined,
    pageNumber: page + 1,
    pageSize,
  });
  const rowCount = getPaginationMeta(data)?.totalCount ?? 0;

  const columns: ColumnDef<AdvanceDto, unknown>[] = [
    { accessorKey: 'employeeName', header: t('columns.employee') },
    {
      accessorKey: 'amount',
      header: t('columns.amount'),
      cell: (info) => `SAR ${Number(info.getValue()).toFixed(2)}`,
    },
    {
      accessorKey: 'advanceDate',
      header: t('columns.advanceDate'),
      cell: (info) => formatDate(info.getValue() as string, i18n.language),
    },
    {
      accessorKey: 'remarks',
      header: t('columns.remarks'),
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
        {canCreate && (
          <Button variant="contained" startIcon={<AddIcon />} onClick={() => setDialogOpen(true)}>
            {t('addAdvance')}
          </Button>
        )}
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
          sx={{ minWidth: 220 }}
        >
          <MenuItem value="">{t('filters.allEmployees')}</MenuItem>
          {(employees ?? []).map((employee) => (
            <MenuItem key={employee.id} value={employee.id}>
              {employeeOptionLabel(employee)}
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

      <CreateAdvanceDialog open={dialogOpen} onClose={() => setDialogOpen(false)} />
    </Box>
  );
}

export default function AdvancesPage() {
  const user = useAppSelector(selectCurrentUser);
  const isFinance = !!user?.roles.some((role) => role === Roles.Administrator || role === Roles.Accountant);

  return isFinance ? <AdvancesManagementView /> : <MyAdvancesView />;
}
