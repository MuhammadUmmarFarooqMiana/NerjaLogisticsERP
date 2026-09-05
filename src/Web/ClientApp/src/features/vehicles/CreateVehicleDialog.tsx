import { Controller, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import {
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  MenuItem,
  Stack,
  TextField,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { usePostApiVehiclesMutation } from '../../api/vehiclesApi';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { VEHICLE_TYPES } from './vehicleTypes';
import { buildVehicleSchema, type VehicleFormValues } from './schemas';

interface CreateVehicleDialogProps {
  open: boolean;
  onClose: () => void;
}

const defaultValues: VehicleFormValues = { registrationNumber: '', vehicleType: 1 };

export function CreateVehicleDialog({ open, onClose }: CreateVehicleDialogProps) {
  const { t } = useTranslation('vehicles');
  const toast = useToast();
  const [createVehicle, { isLoading: saving }] = usePostApiVehiclesMutation();

  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<VehicleFormValues>({
    resolver: zodResolver(buildVehicleSchema(t)),
    defaultValues,
  });

  const handleClose = () => {
    reset(defaultValues);
    onClose();
  };

  const onSubmit = async (values: VehicleFormValues) => {
    try {
      await createVehicle({ createVehicleCommand: values }).unwrap();
      toast.success(t('createSuccess'));
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{t('createDialog.title')}</DialogTitle>
      <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              {...register('registrationNumber')}
              label={t('createDialog.registrationNumber')}
              required
              fullWidth
              error={!!errors.registrationNumber}
              helperText={errors.registrationNumber?.message}
            />
            <Controller
              name="vehicleType"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  onChange={(event) => field.onChange(Number(event.target.value))}
                  select
                  label={t('createDialog.vehicleType')}
                  required
                  fullWidth
                >
                  {VEHICLE_TYPES.map((type) => (
                    <MenuItem key={type.value} value={type.value}>
                      {t(`types.${type.key}`)}
                    </MenuItem>
                  ))}
                </TextField>
              )}
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
            {t('createDialog.submit')}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
