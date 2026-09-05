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
  usePostApiVehiclesByIdAccidentMutation,
  usePutApiVehiclesAccidentByIdMutation,
} from '../../api/vehiclesApi';
import type { AccidentRecordDto } from '../../api/generated/apiSlice';
import { FormDatePicker } from '../../components/shared/FormDatePicker';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { buildAccidentRecordSchema, type AccidentRecordFormValues } from './schemas';

interface AddAccidentRecordDialogProps {
  open: boolean;
  vehicleId: string;
  record?: AccidentRecordDto | null;
  onClose: () => void;
}

const emptyValues: AccidentRecordFormValues = { accidentDate: '', description: '', repairCost: 0 };

export function AddAccidentRecordDialog({ open, vehicleId, record, onClose }: AddAccidentRecordDialogProps) {
  const { t } = useTranslation('vehicles');
  const toast = useToast();
  const [addRecord, { isLoading: adding }] = usePostApiVehiclesByIdAccidentMutation();
  const [updateRecord, { isLoading: updating }] = usePutApiVehiclesAccidentByIdMutation();
  const saving = adding || updating;
  const isEdit = !!record;

  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<AccidentRecordFormValues>({
    resolver: zodResolver(buildAccidentRecordSchema(t)),
    defaultValues: emptyValues,
  });

  useEffect(() => {
    if (open) {
      reset(
        record
          ? {
              accidentDate: record.accidentDate,
              description: record.description,
              repairCost: Number(record.repairCost),
            }
          : emptyValues
      );
    }
  }, [open, record, reset]);

  const handleClose = () => {
    reset(emptyValues);
    onClose();
  };

  const onSubmit = async (values: AccidentRecordFormValues) => {
    try {
      if (isEdit && record) {
        await updateRecord({ id: record.id, updateVehicleAccidentRecordCommand: { id: record.id, ...values } }).unwrap();
        toast.success(t('accidentRecordUpdateSuccess'));
      } else {
        await addRecord({ id: vehicleId, addVehicleAccidentRecordCommand: { vehicleId, ...values } }).unwrap();
        toast.success(t('accidentRecordSuccess'));
      }
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{isEdit ? t('addAccidentDialog.editTitle') : t('addAccidentDialog.title')}</DialogTitle>
      <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <FormDatePicker
              name="accidentDate"
              control={control}
              label={t('addAccidentDialog.accidentDate')}
              required
            />
            <TextField
              {...register('description')}
              label={t('addAccidentDialog.description')}
              required
              fullWidth
              multiline
              rows={2}
              error={!!errors.description}
              helperText={errors.description?.message}
            />
            <TextField
              {...register('repairCost', { valueAsNumber: true })}
              label={t('addAccidentDialog.repairCost')}
              type="number"
              required
              fullWidth
              slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
              error={!!errors.repairCost}
              helperText={errors.repairCost?.message}
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
