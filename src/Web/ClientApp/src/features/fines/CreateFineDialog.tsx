import { zodResolver } from '@hookform/resolvers/zod';
import { Controller, useForm } from 'react-hook-form';
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
import { usePostApiFinesMutation } from '../../api/finesApi';
import { useGetApiEmployeesQuery } from '../../api/employeesApi';
import { FormDatePicker } from '../../components/shared/FormDatePicker';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { employeeOptionLabel } from '../../lib/employeeDisplay';
import { buildFineSchema, type FineFormValues } from './schemas';

interface CreateFineDialogProps {
  open: boolean;
  onClose: () => void;
}

export function CreateFineDialog({ open, onClose }: CreateFineDialogProps) {
  const { t } = useTranslation('fines');
  const toast = useToast();
  const { data: employees } = useGetApiEmployeesQuery({ status: 'Active' });
  const [createFine, { isLoading: saving }] = usePostApiFinesMutation();

  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FineFormValues>({
    resolver: zodResolver(buildFineSchema(t)),
    defaultValues: { employeeId: '', amount: 0, reason: '', fineDate: '' },
  });

  const handleClose = () => {
    reset({ employeeId: '', amount: 0, reason: '', fineDate: '' });
    onClose();
  };

  const onSubmit = async (values: FineFormValues) => {
    try {
      await createFine({
        createFineCommand: {
          employeeId: values.employeeId,
          amount: values.amount,
          reason: values.reason,
          fineDate: values.fineDate,
        },
      }).unwrap();
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
            <Controller
              name="employeeId"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  select
                  label={t('createDialog.employee')}
                  required
                  fullWidth
                  error={!!errors.employeeId}
                  helperText={errors.employeeId?.message}
                >
                  {(employees ?? []).map((employee) => (
                    <MenuItem key={employee.id} value={employee.id}>
                      {employeeOptionLabel(employee)}
                    </MenuItem>
                  ))}
                </TextField>
              )}
            />
            <TextField
              {...register('amount', { valueAsNumber: true })}
              label={t('createDialog.amount')}
              type="number"
              required
              fullWidth
              slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
              error={!!errors.amount}
              helperText={errors.amount?.message}
            />
            <TextField
              {...register('reason')}
              label={t('createDialog.reason')}
              required
              fullWidth
              multiline
              rows={2}
              error={!!errors.reason}
              helperText={errors.reason?.message}
            />
            <FormDatePicker
              name="fineDate"
              control={control}
              label={t('createDialog.fineDate')}
              required
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
