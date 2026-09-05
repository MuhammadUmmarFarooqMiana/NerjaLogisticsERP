import { useMemo, useState } from 'react';
import { Box, Paper, Tab, Tabs, ToggleButton, ToggleButtonGroup, Typography } from '@mui/material';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import { useGetApiDailyOrdersHistoryQuery } from '../../api/dailyOrdersApi';
import type { DailyOrderListItemDto } from '../../api/generated/apiSlice';
import { useAppSelector } from '../../app/hooks';
import { AppDatePicker } from '../../components/shared/AppDatePicker';
import { DataTable } from '../../components/shared/DataTable';
import { StatusBadge } from '../../components/shared/StatusBadge';
import { formatDate, formatDateTime } from '../../lib/formatDate';
import { getPaginationMeta } from '../../lib/pagination';
import { Roles } from '../../lib/roles';
import { selectCurrentUser } from '../auth/authSlice';

type PresetKey = 'last3Days' | 'lastWeek' | 'last30Days' | 'custom';

const PRESET_DAYS_BACK: Record<Exclude<PresetKey, 'custom'>, number> = {
  last3Days: 2,
  lastWeek: 6,
  last30Days: 29,
};

function toIsoDate(date: Date): string {
  return date.toISOString().slice(0, 10);
}

export function OrderHistorySection() {
  const { t, i18n } = useTranslation('dailyOrders');
  const user = useAppSelector(selectCurrentUser);
  const showEmployeeColumn = !!user?.roles.some((role) => role === Roles.Administrator || role === Roles.Supervisor);

  const [preset, setPreset] = useState<PresetKey>('last3Days');
  const [customStart, setCustomStart] = useState('');
  const [customEnd, setCustomEnd] = useState('');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);

  const { startDate, endDate } = useMemo(() => {
    if (preset === 'custom') {
      return { startDate: customStart, endDate: customEnd };
    }
    const end = new Date();
    const start = new Date();
    start.setDate(start.getDate() - PRESET_DAYS_BACK[preset]);
    return { startDate: toIsoDate(start), endDate: toIsoDate(end) };
  }, [preset, customStart, customEnd]);

  const isCustomIncomplete = preset === 'custom' && (!customStart || !customEnd);

  const { data, isLoading, error } = useGetApiDailyOrdersHistoryQuery(
    { startDate, endDate, pageNumber: page + 1, pageSize },
    { skip: isCustomIncomplete }
  );
  const rowCount = getPaginationMeta(data)?.totalCount ?? 0;

  const columns: ColumnDef<DailyOrderListItemDto, unknown>[] = [
    ...(showEmployeeColumn
      ? [{ accessorKey: 'employeeName', header: t('history.columns.name') } as ColumnDef<DailyOrderListItemDto, unknown>]
      : []),
    {
      accessorKey: 'orderDate',
      header: t('history.columns.date'),
      cell: (info) => formatDate(info.getValue() as string, i18n.language),
    },
    { accessorKey: 'completedOrders', header: t('history.columns.completedOrders') },
    {
      accessorKey: 'status',
      header: t('history.columns.status'),
      cell: (info) => <StatusBadge domain="dailyOrderStatus" status={info.getValue() as string} />,
    },
    {
      accessorKey: 'closedAt',
      header: t('history.columns.closedAt'),
      cell: (info) => formatDateTime(info.getValue() as string | null, i18n.language) || '—',
    },
    {
      accessorKey: 'reviewNote',
      header: t('history.columns.reviewNote'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
  ];

  return (
    <Box sx={{ mt: 5 }}>
      <Typography variant="h4" gutterBottom>
        {t('history.title')}
      </Typography>

      <ToggleButtonGroup
        value={preset}
        exclusive
        onChange={(_event, value: PresetKey | null) => {
          if (value) {
            setPreset(value);
            setPage(0);
          }
        }}
        size="small"
        sx={{ mb: 2, flexWrap: 'wrap' }}
      >
        <ToggleButton value="last3Days">{t('history.filters.last3Days')}</ToggleButton>
        <ToggleButton value="lastWeek">{t('history.filters.lastWeek')}</ToggleButton>
        <ToggleButton value="last30Days">{t('history.filters.last30Days')}</ToggleButton>
        <ToggleButton value="custom">{t('history.filters.custom')}</ToggleButton>
      </ToggleButtonGroup>

      {preset === 'custom' && (
        <Box sx={{ display: 'flex', gap: 2, mb: 2, flexWrap: 'wrap' }}>
          <AppDatePicker
            label={t('history.filters.startDate')}
            value={customStart}
            onChange={(value) => {
              setCustomStart(value);
              setPage(0);
            }}
            maxDate={customEnd || undefined}
            slotProps={{ textField: { size: 'small' } }}
          />
          <AppDatePicker
            label={t('history.filters.endDate')}
            value={customEnd}
            onChange={(value) => {
              setCustomEnd(value);
              setPage(0);
            }}
            minDate={customStart || undefined}
            slotProps={{ textField: { size: 'small' } }}
          />
        </Box>
      )}

      <Paper variant="outlined">
        <Tabs value={0} sx={{ borderBottom: 1, borderColor: 'divider', px: 2 }}>
          <Tab label={t('history.tabLabel')} />
        </Tabs>
        <Box sx={{ p: 2 }}>
          <DataTable
            columns={columns}
            data={isCustomIncomplete ? [] : (data ?? [])}
            isLoading={!isCustomIncomplete && isLoading}
            error={error}
            emptyMessage={t('history.empty')}
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
      </Paper>
    </Box>
  );
}
