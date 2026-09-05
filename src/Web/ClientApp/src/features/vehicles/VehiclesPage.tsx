import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Box, Button, Chip, CircularProgress, FormControlLabel, Paper, Stack, Switch, Typography } from '@mui/material';
import AddIcon from '@mui/icons-material/Add';
import DirectionsCarFilledOutlinedIcon from '@mui/icons-material/DirectionsCarFilledOutlined';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import {
  useGetApiVehiclesMeQuery,
  useGetApiVehiclesQuery,
  usePostApiVehiclesByIdActivateMutation,
  usePostApiVehiclesByIdDeactivateMutation,
} from '../../api/vehiclesApi';
import type { VehicleDto } from '../../api/generated/apiSlice';
import { DataTable } from '../../components/shared/DataTable';
import { useAppSelector } from '../../app/hooks';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { getPaginationMeta } from '../../lib/pagination';
import { Roles } from '../../lib/roles';
import { selectCurrentUser } from '../auth/authSlice';
import { AllocateVehicleDialog } from './AllocateVehicleDialog';
import { CreateVehicleDialog } from './CreateVehicleDialog';

function MyVehicleView() {
  const { t } = useTranslation('vehicles');
  const { data: vehicle, isLoading } = useGetApiVehiclesMeQuery();

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('myTitle')}
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        {t('mySubtitle')}
      </Typography>

      {!vehicle ? (
        <Paper variant="outlined" sx={{ p: 4, maxWidth: 480, textAlign: 'center' }}>
          <DirectionsCarFilledOutlinedIcon sx={{ fontSize: 40, color: 'text.disabled', mb: 1 }} />
          <Typography color="text.secondary">{t('myEmpty')}</Typography>
        </Paper>
      ) : (
        <Paper variant="outlined" sx={{ p: 3, maxWidth: 480 }}>
          <Stack spacing={2}>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
              <Typography variant="h6">{vehicle.registrationNumber}</Typography>
              {vehicle.isActive ? (
                <Chip size="small" color="success" label={t('active')} />
              ) : (
                <Chip size="small" label={t('inactive')} />
              )}
            </Stack>
            <Box>
              <Typography variant="caption" color="text.secondary">
                {t('columns.vehicleType')}
              </Typography>
              <Typography>{t(`types.${vehicle.vehicleType}`)}</Typography>
            </Box>
          </Stack>
        </Paper>
      )}
    </Box>
  );
}

function VehiclesManagementView() {
  const { t } = useTranslation('vehicles');
  const navigate = useNavigate();
  const toast = useToast();
  const user = useAppSelector(selectCurrentUser);
  // Accountant can see this whole management view, but Create/Deactivate/
  // Activate/Allocate are backend-restricted to Administrator (Allocate/Return
  // also allow Supervisor, who can't reach this page at all) — hide the
  // actions the backend would 403 on rather than let Accountant click into a
  // dead end.
  const isAdmin = !!user?.roles.some((role) => role === Roles.Administrator);
  const [activeOnly, setActiveOnly] = useState(true);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const { data, isLoading, error } = useGetApiVehiclesQuery({ activeOnly, pageNumber: page + 1, pageSize });
  const rowCount = getPaginationMeta(data)?.totalCount ?? 0;
  const [activate] = usePostApiVehiclesByIdActivateMutation();
  const [deactivate] = usePostApiVehiclesByIdDeactivateMutation();

  const [createOpen, setCreateOpen] = useState(false);
  const [allocateTarget, setAllocateTarget] = useState<VehicleDto | null>(null);

  const handleToggleActive = async (vehicle: VehicleDto) => {
    if (!vehicle.id) return;
    try {
      if (vehicle.isActive) {
        await deactivate({ id: vehicle.id }).unwrap();
        toast.success(t('deactivateSuccess'));
      } else {
        await activate({ id: vehicle.id }).unwrap();
        toast.success(t('activateSuccess'));
      }
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  const columns: ColumnDef<VehicleDto, unknown>[] = [
    { accessorKey: 'registrationNumber', header: t('columns.registrationNumber') },
    {
      accessorKey: 'vehicleType',
      header: t('columns.vehicleType'),
      cell: (info) => t(`types.${info.getValue() as string}`),
    },
    {
      accessorKey: 'isActive',
      header: t('columns.status'),
      cell: (info) =>
        info.getValue() ? (
          <Chip size="small" color="success" label={t('active')} />
        ) : (
          <Chip size="small" label={t('inactive')} />
        ),
    },
    {
      accessorKey: 'assignedEmployeeName',
      header: t('columns.assignedTo'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
    {
      id: 'actions',
      header: '',
      cell: ({ row }) =>
        isAdmin ? (
          <Stack direction="row" spacing={1} onClick={(event) => event.stopPropagation()}>
            <Button
              size="small"
              color={row.original.isActive ? 'error' : 'primary'}
              onClick={() => void handleToggleActive(row.original)}
            >
              {row.original.isActive ? t('deactivate') : t('activate')}
            </Button>
            {row.original.isActive && !row.original.assignedEmployeeName && (
              <Button size="small" onClick={() => setAllocateTarget(row.original)}>
                {t('allocate')}
              </Button>
            )}
          </Stack>
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
        {isAdmin && (
          <Button variant="contained" startIcon={<AddIcon />} onClick={() => setCreateOpen(true)}>
            {t('addVehicle')}
          </Button>
        )}
      </Stack>

      <Stack direction="row" sx={{ my: 2 }}>
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
        onRowClick={(row) => navigate(`/vehicles/${row.id}`)}
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

      <CreateVehicleDialog open={createOpen} onClose={() => setCreateOpen(false)} />
      <AllocateVehicleDialog
        open={!!allocateTarget}
        vehicle={allocateTarget}
        onClose={() => setAllocateTarget(null)}
      />
    </Box>
  );
}

export default function VehiclesPage() {
  const user = useAppSelector(selectCurrentUser);
  const isFleetManager = !!user?.roles.some((role) => role === Roles.Administrator || role === Roles.Accountant);

  return isFleetManager ? <VehiclesManagementView /> : <MyVehicleView />;
}
