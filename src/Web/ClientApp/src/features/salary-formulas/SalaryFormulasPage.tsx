import { useState } from 'react';
import { Box, Button, Chip, FormControlLabel, MenuItem, Stack, Switch, TextField, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import {
  useGetApiSalaryFormulasQuery,
  usePostApiSalaryFormulasByIdDeactivateMutation,
} from '../../api/salaryFormulasApi';
import { useGetApiPlatformsQuery } from '../../api/generated/apiSlice';
import type { SalaryFormulaDto } from '../../api/generated/apiSlice';
import { ConfirmDialog } from '../../components/shared/ConfirmDialog';
import { DataTable } from '../../components/shared/DataTable';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { formatDate } from '../../lib/formatDate';
import { getPaginationMeta } from '../../lib/pagination';
import { CreateSalaryFormulaDialog } from './CreateSalaryFormulaDialog';
import { SalaryFormulaDetailDialog } from './SalaryFormulaDetailDialog';

export default function SalaryFormulasPage() {
  const { t, i18n } = useTranslation('salaryFormulas');
  const toast = useToast();
  const { data: platforms } = useGetApiPlatformsQuery();
  const [platformId, setPlatformId] = useState('');
  const [activeOnly, setActiveOnly] = useState(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [deactivateTarget, setDeactivateTarget] = useState<SalaryFormulaDto | null>(null);
  const [detailTarget, setDetailTarget] = useState<string | null>(null);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);

  const { data, isLoading, error } = useGetApiSalaryFormulasQuery({
    platformId: platformId || undefined,
    activeOnly,
    pageNumber: page + 1,
    pageSize,
  });
  const rowCount = getPaginationMeta(data)?.totalCount ?? 0;
  const [deactivate, { isLoading: deactivating }] = usePostApiSalaryFormulasByIdDeactivateMutation();

  const handleDeactivate = async () => {
    if (!deactivateTarget?.id) return;
    try {
      await deactivate({ id: deactivateTarget.id }).unwrap();
      toast.success(t('deactivateSuccess'));
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    } finally {
      setDeactivateTarget(null);
    }
  };

  const columns: ColumnDef<SalaryFormulaDto, unknown>[] = [
    {
      accessorKey: 'platformName',
      header: t('columns.platform'),
      cell: (info) => (info.getValue() as string | null) ?? t('genericDefault'),
    },
    {
      accessorKey: 'formulaType',
      header: t('columns.formulaType'),
      cell: (info) => t(`formulaTypes.${info.getValue() as string}`),
    },
    {
      accessorKey: 'fixedMonthlyAmount',
      header: t('columns.fixedMonthlyAmount'),
      cell: (info) => {
        const value = info.getValue();
        return value != null ? `SAR ${Number(value).toFixed(2)}` : '—';
      },
    },
    {
      accessorKey: 'effectiveFrom',
      header: t('columns.effectiveFrom'),
      cell: (info) => formatDate(info.getValue() as string, i18n.language),
    },
    {
      accessorKey: 'effectiveTo',
      header: t('columns.status'),
      cell: (info) =>
        info.getValue() == null ? (
          <Chip size="small" color="success" label={t('active')} />
        ) : (
          <Chip size="small" label={formatDate(info.getValue() as string, i18n.language)} />
        ),
    },
    {
      id: 'actions',
      header: '',
      cell: ({ row }) =>
        row.original.effectiveTo == null ? (
          <Button
            size="small"
            color="error"
            onClick={(event) => {
              event.stopPropagation();
              setDeactivateTarget(row.original);
            }}
          >
            {t('deactivate')}
          </Button>
        ) : null,
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
          {t('addFormula')}
        </Button>
      </Stack>

      <Stack direction="row" spacing={2} sx={{ my: 2, alignItems: 'center', flexWrap: 'wrap' }}>
        <TextField
          select
          size="small"
          label={t('filters.platform')}
          value={platformId}
          onChange={(event) => {
            setPlatformId(event.target.value);
            setPage(0);
          }}
          sx={{ minWidth: 220 }}
        >
          <MenuItem value="">{t('filters.allPlatforms')}</MenuItem>
          {(platforms ?? []).map((platform) => (
            <MenuItem key={platform.id} value={platform.id}>
              {platform.name}
            </MenuItem>
          ))}
        </TextField>
        <FormControlLabel
          control={
            <Switch
              checked={activeOnly}
              onChange={(event) => {
                setActiveOnly(event.target.checked);
                setPage(0);
              }}
            />
          }
          label={t('filters.activeOnly')}
        />
      </Stack>

      <DataTable
        columns={columns}
        data={data ?? []}
        isLoading={isLoading}
        error={error}
        emptyMessage={t('empty')}
        onRowClick={(row) => row.id && setDetailTarget(row.id)}
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

      <CreateSalaryFormulaDialog open={dialogOpen} onClose={() => setDialogOpen(false)} />

      <SalaryFormulaDetailDialog
        open={!!detailTarget}
        formulaId={detailTarget}
        onClose={() => setDetailTarget(null)}
      />

      <ConfirmDialog
        open={!!deactivateTarget}
        title={t('deactivateConfirmTitle')}
        description={t('deactivateConfirmDescription')}
        destructive
        loading={deactivating}
        onConfirm={() => void handleDeactivate()}
        onCancel={() => setDeactivateTarget(null)}
      />
    </Box>
  );
}
