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
import { usePostApiLeaveRequestsMutation } from '../../api/leaveRequestsApi';
import { FormDatePicker } from '../../components/shared/FormDatePicker';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { buildLeaveRequestSchema, type LeaveRequestFormValues } from './schemas';

interface SubmitLeaveRequestDialogProps {
  open: boolean;
  onClose: () => void;
}

const defaultValues: LeaveRequestFormValues = { startDate: '', endDate: '', reason: '' };

export function SubmitLeaveRequestDialog({ open, onClose }: SubmitLeaveRequestDialogProps) {
  const { t } = useTranslation('leaveRequests');
  const toast = useToast();
  const [submit, { isLoading: saving }] = usePostApiLeaveRequestsMutation();

  const {
    control,
    register,
    handleSubmit,
    reset,
    watch,
  } = useForm<LeaveRequestFormValues>({
    resolver: zodResolver(buildLeaveRequestSchema(t)),
    defaultValues,
  });
  const startDate = watch('startDate');
  const endDate = watch('endDate');

  const handleClose = () => {
    reset(defaultValues);
    onClose();
  };

  const onSubmit = async (values: LeaveRequestFormValues) => {
    try {
      await submit({
        submitLeaveRequestCommand: {
          startDate: values.startDate,
          endDate: values.endDate,
          reason: values.reason || null,
        },
      }).unwrap();
      toast.success(t('submitSuccess'));
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{t('submitDialog.title')}</DialogTitle>
      <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <FormDatePicker
              name="startDate"
              control={control}
              label={t('submitDialog.startDate')}
              required
              maxDate={endDate || undefined}
            />
            <FormDatePicker
              name="endDate"
              control={control}
              label={t('submitDialog.endDate')}
              required
              minDate={startDate || undefined}
            />
            <TextField
              {...register('reason')}
              label={t('submitDialog.reason')}
              fullWidth
              multiline
              rows={3}
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
            {t('submitDialog.submit')}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
