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
import { usePostApiMonthlySummariesGenerateMutation } from '../../api/monthlySummariesApi';
import { useGetApiEmployeesQuery } from '../../api/employeesApi';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { employeeOptionLabel } from '../../lib/employeeDisplay';
import { buildGenerateSummarySchema, type GenerateSummaryFormValues } from './schemas';

interface GenerateSummaryDialogProps {
  open: boolean;
  onClose: () => void;
}

const now = new Date();

export function GenerateSummaryDialog({ open, onClose }: GenerateSummaryDialogProps) {
  const { t } = useTranslation('monthlySummaries');
  const toast = useToast();
  const { data: employees } = useGetApiEmployeesQuery({ status: 'Active' });
  const [generate, { isLoading: saving }] = usePostApiMonthlySummariesGenerateMutation();

  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<GenerateSummaryFormValues>({
    resolver: zodResolver(buildGenerateSummarySchema(t)),
    defaultValues: { employeeId: '', year: now.getFullYear(), month: now.getMonth() + 1 },
  });

  const handleClose = () => {
    reset({ employeeId: '', year: now.getFullYear(), month: now.getMonth() + 1 });
    onClose();
  };

  const onSubmit = async (values: GenerateSummaryFormValues) => {
    try {
      await generate({
        generateMonthlySummaryCommand: {
          employeeId: values.employeeId,
          year: values.year,
          month: values.month,
        },
      }).unwrap();
      toast.success(t('generateSuccess'));
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{t('generateDialog.title')}</DialogTitle>
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
                  label={t('generateDialog.employee')}
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
              {...register('year', { valueAsNumber: true })}
              label={t('generateDialog.year')}
              type="number"
              required
              fullWidth
              error={!!errors.year}
              helperText={errors.year?.message}
            />
            <TextField
              {...register('month', { valueAsNumber: true })}
              label={t('generateDialog.month')}
              type="number"
              required
              fullWidth
              slotProps={{ htmlInput: { min: 1, max: 12 } }}
              error={!!errors.month}
              helperText={errors.month?.message}
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
            {t('generateDialog.submit')}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
