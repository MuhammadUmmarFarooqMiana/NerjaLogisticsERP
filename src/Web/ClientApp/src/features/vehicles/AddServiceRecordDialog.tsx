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
  usePostApiVehiclesByIdServiceMutation,
  usePutApiVehiclesServiceByIdMutation,
} from '../../api/vehiclesApi';
import type { ServiceRecordDto } from '../../api/generated/apiSlice';
import { FormDatePicker } from '../../components/shared/FormDatePicker';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { buildServiceRecordSchema, type ServiceRecordFormValues } from './schemas';

interface AddServiceRecordDialogProps {
  open: boolean;
  vehicleId: string;
  record?: ServiceRecordDto | null;
  onClose: () => void;
}

const emptyValues: ServiceRecordFormValues = { serviceDate: '', odometer: 0, description: '', cost: 0 };

export function AddServiceRecordDialog({ open, vehicleId, record, onClose }: AddServiceRecordDialogProps) {
  const { t } = useTranslation('vehicles');
  const toast = useToast();
  const [addRecord, { isLoading: adding }] = usePostApiVehiclesByIdServiceMutation();
  const [updateRecord, { isLoading: updating }] = usePutApiVehiclesServiceByIdMutation();
  const saving = adding || updating;
  const isEdit = !!record;

  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ServiceRecordFormValues>({
    resolver: zodResolver(buildServiceRecordSchema(t)),
    defaultValues: emptyValues,
  });

  useEffect(() => {
    if (open) {
      reset(
        record
          ? {
              serviceDate: record.serviceDate,
              odometer: Number(record.odometer),
              description: record.description,
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

  const onSubmit = async (values: ServiceRecordFormValues) => {
    try {
      if (isEdit && record) {
        await updateRecord({ id: record.id, updateVehicleServiceRecordCommand: { id: record.id, ...values } }).unwrap();
        toast.success(t('serviceRecordUpdateSuccess'));
      } else {
        await addRecord({ id: vehicleId, addVehicleServiceRecordCommand: { vehicleId, ...values } }).unwrap();
        toast.success(t('serviceRecordSuccess'));
      }
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{isEdit ? t('addServiceDialog.editTitle') : t('addServiceDialog.title')}</DialogTitle>
      <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <FormDatePicker
              name="serviceDate"
              control={control}
              label={t('addServiceDialog.serviceDate')}
              required
            />
            <TextField
              {...register('odometer', { valueAsNumber: true })}
              label={t('addServiceDialog.odometer')}
              type="number"
              required
              fullWidth
              slotProps={{ htmlInput: { min: 0 } }}
              error={!!errors.odometer}
              helperText={errors.odometer?.message}
            />
            <TextField
              {...register('description')}
              label={t('addServiceDialog.description')}
              required
              fullWidth
              multiline
              rows={2}
              error={!!errors.description}
              helperText={errors.description?.message}
            />
            <TextField
              {...register('cost', { valueAsNumber: true })}
              label={t('addServiceDialog.cost')}
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
