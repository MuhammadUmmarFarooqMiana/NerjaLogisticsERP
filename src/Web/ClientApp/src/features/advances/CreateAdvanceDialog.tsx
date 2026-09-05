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
import { usePostApiAdvancesMutation } from '../../api/advancesApi';
import { useGetApiEmployeesQuery } from '../../api/employeesApi';
import { FormDatePicker } from '../../components/shared/FormDatePicker';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { employeeOptionLabel } from '../../lib/employeeDisplay';
import { buildAdvanceSchema, type AdvanceFormValues } from './schemas';

interface CreateAdvanceDialogProps {
  open: boolean;
  onClose: () => void;
}

export function CreateAdvanceDialog({ open, onClose }: CreateAdvanceDialogProps) {
  const { t } = useTranslation('advances');
  const toast = useToast();
  const { data: employees } = useGetApiEmployeesQuery({ status: 'Active' });
  const [createAdvance, { isLoading: saving }] = usePostApiAdvancesMutation();

  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<AdvanceFormValues>({
    resolver: zodResolver(buildAdvanceSchema(t)),
    defaultValues: { employeeId: '', amount: 0, advanceDate: '', remarks: '' },
  });

  const handleClose = () => {
    reset({ employeeId: '', amount: 0, advanceDate: '', remarks: '' });
    onClose();
  };

  const onSubmit = async (values: AdvanceFormValues) => {
    try {
      await createAdvance({
        createAdvanceCommand: {
          employeeId: values.employeeId,
          amount: values.amount,
          advanceDate: values.advanceDate,
          remarks: values.remarks || null,
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
            <FormDatePicker
              name="advanceDate"
              control={control}
              label={t('createDialog.advanceDate')}
              required
            />
            <TextField
              {...register('remarks')}
              label={t('createDialog.remarks')}
              fullWidth
              multiline
              rows={2}
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
