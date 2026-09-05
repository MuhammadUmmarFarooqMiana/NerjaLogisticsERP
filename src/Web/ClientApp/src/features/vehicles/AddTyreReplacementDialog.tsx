import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import {
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Stack,
  TextField,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import {
  usePostApiVehiclesByIdTyreReplacementMutation,
  usePutApiVehiclesTyreReplacementByIdMutation,
} from '../../api/vehiclesApi';
import type { TyreReplacementRecordDto } from '../../api/generated/apiSlice';
import { FormDatePicker } from '../../components/shared/FormDatePicker';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { buildTyreReplacementSchema, type TyreReplacementFormValues } from './schemas';

interface AddTyreReplacementDialogProps {
  open: boolean;
  vehicleId: string;
  record?: TyreReplacementRecordDto | null;
  onClose: () => void;
}

const emptyValues: TyreReplacementFormValues = { replacementDate: '', odometer: 0, numberOfTyres: 4, cost: 0 };

export function AddTyreReplacementDialog({ open, vehicleId, record, onClose }: AddTyreReplacementDialogProps) {
  const { t } = useTranslation('vehicles');
  const toast = useToast();
  const [addRecord, { isLoading: adding }] = usePostApiVehiclesByIdTyreReplacementMutation();
  const [updateRecord, { isLoading: updating }] = usePutApiVehiclesTyreReplacementByIdMutation();
  const saving = adding || updating;
  const isEdit = !!record;

  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<TyreReplacementFormValues>({
    resolver: zodResolver(buildTyreReplacementSchema(t)),
    defaultValues: emptyValues,
  });

  useEffect(() => {
    if (open) {
      reset(
        record
          ? {
              replacementDate: record.replacementDate,
              odometer: Number(record.odometer),
              numberOfTyres: Number(record.numberOfTyres),
              cost: Number(record.cost),
            }
          : emptyValues
      );
    }
  }, [open, record, reset]);

  const handleClose = () => {
    reset(emptyValues);
    onClose();
  };

  const onSubmit = async (values: TyreReplacementFormValues) => {
    try {
      if (isEdit && record) {
        await updateRecord({
          id: record.id,
          updateVehicleTyreReplacementRecordCommand: { id: record.id, ...values },
        }).unwrap();
        toast.success(t('tyreReplacementUpdateSuccess'));
      } else {
        await addRecord({
          id: vehicleId,
          addVehicleTyreReplacementRecordCommand: { vehicleId, ...values },
        }).unwrap();
        toast.success(t('tyreReplacementSuccess'));
      }
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{isEdit ? t('addTyreDialog.editTitle') : t('addTyreDialog.title')}</DialogTitle>
      <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <FormDatePicker
              name="replacementDate"
              control={control}
              label={t('addTyreDialog.replacementDate')}
              required
            />
            <TextField
              {...register('odometer', { valueAsNumber: true })}
              label={t('addTyreDialog.odometer')}
              type="number"
              required
              fullWidth
              slotProps={{ htmlInput: { min: 0 } }}
              error={!!errors.odometer}
              helperText={errors.odometer?.message}
            />
            <TextField
              {...register('numberOfTyres', { valueAsNumber: true })}
              label={t('addTyreDialog.numberOfTyres')}
              type="number"
              required
              fullWidth
              slotProps={{ htmlInput: { min: 1, max: 6 } }}
              error={!!errors.numberOfTyres}
              helperText={errors.numberOfTyres?.message}
            />
            <TextField
              {...register('cost', { valueAsNumber: true })}
              label={t('addTyreDialog.cost')}
              type="number"
              required
              fullWidth
              slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
              error={!!errors.cost}
              helperText={errors.cost?.message}
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>{t('common:actions.cancel')}</Button>
          <Button
            type="submit"
            variant="contained"
            disabled={saving}
            startIcon={saving ? <CircularProgress size={18} color="inherit" /> : undefined}
          >
            {t('common:actions.confirm')}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
