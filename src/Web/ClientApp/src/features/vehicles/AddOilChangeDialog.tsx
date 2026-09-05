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
  usePostApiVehiclesByIdOilChangeMutation,
  usePutApiVehiclesOilChangeByIdMutation,
} from '../../api/vehiclesApi';
import type { OilChangeRecordDto } from '../../api/generated/apiSlice';
import { FormDatePicker } from '../../components/shared/FormDatePicker';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { buildOilChangeSchema, type OilChangeFormValues } from './schemas';

interface AddOilChangeDialogProps {
  open: boolean;
  vehicleId: string;
  record?: OilChangeRecordDto | null;
  onClose: () => void;
}

const emptyValues: OilChangeFormValues = { changeDate: '', odometer: 0, cost: 0 };

export function AddOilChangeDialog({ open, vehicleId, record, onClose }: AddOilChangeDialogProps) {
  const { t } = useTranslation('vehicles');
  const toast = useToast();
  const [addRecord, { isLoading: adding }] = usePostApiVehiclesByIdOilChangeMutation();
  const [updateRecord, { isLoading: updating }] = usePutApiVehiclesOilChangeByIdMutation();
  const saving = adding || updating;
  const isEdit = !!record;

  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<OilChangeFormValues>({
    resolver: zodResolver(buildOilChangeSchema(t)),
    defaultValues: emptyValues,
  });

  useEffect(() => {
    if (open) {
      reset(
        record
          ? { changeDate: record.changeDate, odometer: Number(record.odometer), cost: Number(record.cost) }
          : emptyValues
      );
    }
  }, [open, record, reset]);

  const handleClose = () => {
    reset(emptyValues);
    onClose();
  };

  const onSubmit = async (values: OilChangeFormValues) => {
    try {
      if (isEdit && record) {
        await updateRecord({ id: record.id, updateVehicleOilChangeRecordCommand: { id: record.id, ...values } }).unwrap();
        toast.success(t('oilChangeUpdateSuccess'));
      } else {
        await addRecord({ id: vehicleId, addVehicleOilChangeRecordCommand: { vehicleId, ...values } }).unwrap();
        toast.success(t('oilChangeSuccess'));
      }
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{isEdit ? t('addOilChangeDialog.editTitle') : t('addOilChangeDialog.title')}</DialogTitle>
      <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <FormDatePicker
              name="changeDate"
              control={control}
              label={t('addOilChangeDialog.changeDate')}
              required
            />
            <TextField
              {...register('odometer', { valueAsNumber: true })}
              label={t('addOilChangeDialog.odometer')}
              type="number"
              required
              fullWidth
              slotProps={{ htmlInput: { min: 0 } }}
              error={!!errors.odometer}
              helperText={errors.odometer?.message}
            />
            <TextField
              {...register('cost', { valueAsNumber: true })}
              label={t('addOilChangeDialog.cost')}
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
