import { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import AddIcon from '@mui/icons-material/Add';
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutlineOutlined';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import { Alert, Box, Button, Chip, CircularProgress, IconButton, Paper, Stack, Typography } from '@mui/material';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import {
  useDeleteApiVehiclesAccidentByIdMutation,
  useDeleteApiVehiclesAllocationsByIdMutation,
  useDeleteApiVehiclesOilChangeByIdMutation,
  useDeleteApiVehiclesServiceByIdMutation,
  useDeleteApiVehiclesTyreReplacementByIdMutation,
  useGetApiVehiclesByIdHistoryQuery,
  useGetApiVehiclesByIdQuery,
} from '../../api/vehiclesApi';
import type {
  AccidentRecordDto,
  AllocationRecordDto,
  OilChangeRecordDto,
  ServiceRecordDto,
  TyreReplacementRecordDto,
} from '../../api/generated/apiSlice';
import { useAppSelector } from '../../app/hooks';
import { ConfirmDialog } from '../../components/shared/ConfirmDialog';
import { DataTable } from '../../components/shared/DataTable';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { formatDate } from '../../lib/formatDate';
import { Roles } from '../../lib/roles';
import { selectCurrentUser } from '../auth/authSlice';
import { AddAccidentRecordDialog } from './AddAccidentRecordDialog';
import { AddOilChangeDialog } from './AddOilChangeDialog';
import { AddServiceRecordDialog } from './AddServiceRecordDialog';
import { AddTyreReplacementDialog } from './AddTyreReplacementDialog';
import { EditAllocationDialog } from './EditAllocationDialog';

type DeleteTarget = { kind: 'allocation' | 'service' | 'oilChange' | 'tyre' | 'accident'; id: string } | null;

export default function VehicleDetailPage() {
  const { t, i18n } = useTranslation('vehicles');
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const toast = useToast();
  const user = useAppSelector(selectCurrentUser);
  const isAdmin = !!user?.roles.some((role) => role === Roles.Administrator);

  const { data: vehicle, isLoading, error } = useGetApiVehiclesByIdQuery({ id: id ?? '' }, { skip: !id });
  const {
    data: history,
    isLoading: historyLoading,
    error: historyError,
  } = useGetApiVehiclesByIdHistoryQuery({ id: id ?? '' }, { skip: !id });

  const [serviceDialog, setServiceDialog] = useState<{ open: boolean; record: ServiceRecordDto | null }>({
    open: false,
    record: null,
  });
  const [oilDialog, setOilDialog] = useState<{ open: boolean; record: OilChangeRecordDto | null }>({
    open: false,
    record: null,
  });
  const [tyreDialog, setTyreDialog] = useState<{ open: boolean; record: TyreReplacementRecordDto | null }>({
    open: false,
    record: null,
  });
  const [accidentDialog, setAccidentDialog] = useState<{ open: boolean; record: AccidentRecordDto | null }>({
    open: false,
    record: null,
  });
  const [allocationDialog, setAllocationDialog] = useState<AllocationRecordDto | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<DeleteTarget>(null);

  const [deleteAllocation, { isLoading: deletingAllocation }] = useDeleteApiVehiclesAllocationsByIdMutation();
  const [deleteService, { isLoading: deletingService }] = useDeleteApiVehiclesServiceByIdMutation();
  const [deleteOilChange, { isLoading: deletingOilChange }] = useDeleteApiVehiclesOilChangeByIdMutation();
  const [deleteTyre, { isLoading: deletingTyre }] = useDeleteApiVehiclesTyreReplacementByIdMutation();
  const [deleteAccident, { isLoading: deletingAccident }] = useDeleteApiVehiclesAccidentByIdMutation();

  const deleting =
    deletingAllocation || deletingService || deletingOilChange || deletingTyre || deletingAccident;

  const handleDelete = async () => {
    if (!deleteTarget) return;
    try {
      switch (deleteTarget.kind) {
        case 'allocation':
          await deleteAllocation({ id: deleteTarget.id }).unwrap();
          break;
        case 'service':
          await deleteService({ id: deleteTarget.id }).unwrap();
          break;
        case 'oilChange':
          await deleteOilChange({ id: deleteTarget.id }).unwrap();
          break;
        case 'tyre':
          await deleteTyre({ id: deleteTarget.id }).unwrap();
          break;
        case 'accident':
          await deleteAccident({ id: deleteTarget.id }).unwrap();
          break;
      }
      toast.success(t('detail.deleteSuccess'));
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    } finally {
      setDeleteTarget(null);
    }
  };

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error || !vehicle) {
    return <Alert severity="error">{t('detail.genericError')}</Alert>;
  }

  const allocationColumns: ColumnDef<AllocationRecordDto, unknown>[] = [
    { accessorKey: 'employeeName', header: t('detail.columns.employee') },
    {
      accessorKey: 'assignedDate',
      header: t('detail.columns.assignedDate'),
      cell: (i) => formatDate(i.getValue() as string, i18n.language),
    },
    {
      accessorKey: 'returnedDate',
      header: t('detail.columns.returnedDate'),
      cell: (i) => {
        const value = i.getValue() as string | null;
        return value ? (
          formatDate(value, i18n.language)
        ) : (
          <Chip size="small" color="info" label={t('detail.currentlyAllocated')} />
        );
      },
    },
    ...(isAdmin
      ? [
          {
            id: 'actions',
            header: '',
            cell: ({ row }: { row: { original: AllocationRecordDto } }) => (
              <Stack direction="row" spacing={0.5}>
                <IconButton size="small" aria-label={t('detail.edit')} onClick={() => setAllocationDialog(row.original)}>
                  <EditOutlinedIcon fontSize="small" />
                </IconButton>
                <IconButton
                  size="small"
                  aria-label={t('detail.delete')}
                  onClick={() => setDeleteTarget({ kind: 'allocation', id: row.original.id })}
                >
                  <DeleteOutlineIcon fontSize="small" />
                </IconButton>
              </Stack>
            ),
          } satisfies ColumnDef<AllocationRecordDto, unknown>,
        ]
      : []),
  ];

  const serviceColumns: ColumnDef<ServiceRecordDto, unknown>[] = [
    { accessorKey: 'serviceDate', header: t('detail.columns.date'), cell: (i) => formatDate(i.getValue() as string, i18n.language) },
    { accessorKey: 'odometer', header: t('detail.columns.odometer') },
    { accessorKey: 'description', header: t('detail.columns.description') },
    { accessorKey: 'cost', header: t('detail.columns.cost'), cell: (i) => `SAR ${Number(i.getValue()).toFixed(2)}` },
    {
      id: 'actions',
      header: '',
      cell: ({ row }) => (
        <Stack direction="row" spacing={0.5}>
          <IconButton
            size="small"
            aria-label={t('detail.edit')}
            onClick={() => setServiceDialog({ open: true, record: row.original })}
          >
            <EditOutlinedIcon fontSize="small" />
          </IconButton>
          {isAdmin && (
            <IconButton
              size="small"
              aria-label={t('detail.delete')}
              onClick={() => setDeleteTarget({ kind: 'service', id: row.original.id })}
            >
              <DeleteOutlineIcon fontSize="small" />
            </IconButton>
          )}
        </Stack>
      ),
    },
  ];

  const oilColumns: ColumnDef<OilChangeRecordDto, unknown>[] = [
    { accessorKey: 'changeDate', header: t('detail.columns.date'), cell: (i) => formatDate(i.getValue() as string, i18n.language) },
    { accessorKey: 'odometer', header: t('detail.columns.odometer') },
    { accessorKey: 'cost', header: t('detail.columns.cost'), cell: (i) => `SAR ${Number(i.getValue()).toFixed(2)}` },
    {
      id: 'actions',
      header: '',
      cell: ({ row }) => (
        <Stack direction="row" spacing={0.5}>
          <IconButton
            size="small"
            aria-label={t('detail.edit')}
            onClick={() => setOilDialog({ open: true, record: row.original })}
          >
            <EditOutlinedIcon fontSize="small" />
          </IconButton>
          {isAdmin && (
            <IconButton
              size="small"
              aria-label={t('detail.delete')}
              onClick={() => setDeleteTarget({ kind: 'oilChange', id: row.original.id })}
            >
              <DeleteOutlineIcon fontSize="small" />
            </IconButton>
          )}
        </Stack>
      ),
    },
  ];

  const tyreColumns: ColumnDef<TyreReplacementRecordDto, unknown>[] = [
    { accessorKey: 'replacementDate', header: t('detail.columns.date'), cell: (i) => formatDate(i.getValue() as string, i18n.language) },
    { accessorKey: 'odometer', header: t('detail.columns.odometer') },
    { accessorKey: 'numberOfTyres', header: t('detail.columns.numberOfTyres') },
    { accessorKey: 'cost', header: t('detail.columns.cost'), cell: (i) => `SAR ${Number(i.getValue()).toFixed(2)}` },
    {
      id: 'actions',
      header: '',
      cell: ({ row }) => (
        <Stack direction="row" spacing={0.5}>
          <IconButton
            size="small"
            aria-label={t('detail.edit')}
            onClick={() => setTyreDialog({ open: true, record: row.original })}
          >
            <EditOutlinedIcon fontSize="small" />
          </IconButton>
          {isAdmin && (
            <IconButton
              size="small"
              aria-label={t('detail.delete')}
              onClick={() => setDeleteTarget({ kind: 'tyre', id: row.original.id })}
            >
              <DeleteOutlineIcon fontSize="small" />
            </IconButton>
          )}
        </Stack>
      ),
    },
  ];

  const accidentColumns: ColumnDef<AccidentRecordDto, unknown>[] = [
    { accessorKey: 'accidentDate', header: t('detail.columns.date'), cell: (i) => formatDate(i.getValue() as string, i18n.language) },
    { accessorKey: 'description', header: t('detail.columns.description') },
    { accessorKey: 'repairCost', header: t('detail.columns.cost'), cell: (i) => `SAR ${Number(i.getValue()).toFixed(2)}` },
    {
      id: 'actions',
      header: '',
      cell: ({ row }) => (
        <Stack direction="row" spacing={0.5}>
          <IconButton
            size="small"
            aria-label={t('detail.edit')}
            onClick={() => setAccidentDialog({ open: true, record: row.original })}
          >
            <EditOutlinedIcon fontSize="small" />
          </IconButton>
          {isAdmin && (
            <IconButton
              size="small"
              aria-label={t('detail.delete')}
              onClick={() => setDeleteTarget({ kind: 'accident', id: row.original.id })}
            >
              <DeleteOutlineIcon fontSize="small" />
            </IconButton>
          )}
        </Stack>
      ),
    },
  ];

  return (
    <Box>
      <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/vehicles')} sx={{ mb: 2 }}>
        {t('detail.backToList')}
      </Button>

      <Paper variant="outlined" sx={{ p: 3, mb: 3 }}>
        <Stack direction="row" spacing={2} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <Typography variant="h4">{vehicle.registrationNumber}</Typography>
          <Chip size="small" label={vehicle.vehicleType ? t(`types.${vehicle.vehicleType}`) : ''} />
          <Chip
            size="small"
            color={vehicle.isActive ? 'success' : 'default'}
            label={vehicle.isActive ? t('active') : t('inactive')}
          />
        </Stack>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
          {vehicle.assignedEmployeeName
            ? t('detail.assignedTo', { name: vehicle.assignedEmployeeName })
            : t('detail.unassigned')}
        </Typography>
      </Paper>

      <Stack spacing={4}>
        <Box>
          <Typography variant="h6" sx={{ mb: 1 }}>
            {t('detail.allocations')}
          </Typography>
          <DataTable
            columns={allocationColumns}
            data={history?.allocations ?? []}
            isLoading={historyLoading}
            error={historyError}
            emptyMessage={t('detail.empty')}
          />
        </Box>

        <Box>
          <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
            <Typography variant="h6">{t('detail.serviceHistory')}</Typography>
            <Button size="small" startIcon={<AddIcon />} onClick={() => setServiceDialog({ open: true, record: null })}>
              {t('detail.addRecord')}
            </Button>
          </Stack>
          <DataTable
            columns={serviceColumns}
            data={history?.serviceHistory ?? []}
            isLoading={historyLoading}
            error={historyError}
            emptyMessage={t('detail.empty')}
          />
        </Box>

        <Box>
          <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
            <Typography variant="h6">{t('detail.oilChanges')}</Typography>
            <Button size="small" startIcon={<AddIcon />} onClick={() => setOilDialog({ open: true, record: null })}>
              {t('detail.addRecord')}
            </Button>
          </Stack>
          <DataTable
            columns={oilColumns}
            data={history?.oilChanges ?? []}
            isLoading={historyLoading}
            error={historyError}
            emptyMessage={t('detail.empty')}
          />
        </Box>

        <Box>
          <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
            <Typography variant="h6">{t('detail.tyreReplacements')}</Typography>
            <Button size="small" startIcon={<AddIcon />} onClick={() => setTyreDialog({ open: true, record: null })}>
              {t('detail.addRecord')}
            </Button>
          </Stack>
          <DataTable
            columns={tyreColumns}
            data={history?.tyreReplacements ?? []}
            isLoading={historyLoading}
            error={historyError}
            emptyMessage={t('detail.empty')}
          />
        </Box>

        <Box>
          <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
            <Typography variant="h6">{t('detail.accidentHistory')}</Typography>
            <Button size="small" startIcon={<AddIcon />} onClick={() => setAccidentDialog({ open: true, record: null })}>
              {t('detail.addRecord')}
            </Button>
          </Stack>
          <DataTable
            columns={accidentColumns}
            data={history?.accidentHistory ?? []}
            isLoading={historyLoading}
            error={historyError}
            emptyMessage={t('detail.empty')}
          />
        </Box>
      </Stack>

      {id && (
        <>
          <AddServiceRecordDialog
            open={serviceDialog.open}
            vehicleId={id}
            record={serviceDialog.record}
            onClose={() => setServiceDialog({ open: false, record: null })}
          />
          <AddOilChangeDialog
            open={oilDialog.open}
            vehicleId={id}
            record={oilDialog.record}
            onClose={() => setOilDialog({ open: false, record: null })}
          />
          <AddTyreReplacementDialog
            open={tyreDialog.open}
            vehicleId={id}
            record={tyreDialog.record}
            onClose={() => setTyreDialog({ open: false, record: null })}
          />
          <AddAccidentRecordDialog
            open={accidentDialog.open}
            vehicleId={id}
            record={accidentDialog.record}
            onClose={() => setAccidentDialog({ open: false, record: null })}
          />
        </>
      )}

      <EditAllocationDialog
        open={!!allocationDialog}
        allocation={allocationDialog}
        onClose={() => setAllocationDialog(null)}
      />

      <ConfirmDialog
        open={!!deleteTarget}
        title={t('detail.deleteConfirmTitle')}
        description={t('detail.deleteConfirmDescription')}
        destructive
        loading={deleting}
        onConfirm={() => void handleDelete()}
        onCancel={() => setDeleteTarget(null)}
      />
    </Box>
  );
}
