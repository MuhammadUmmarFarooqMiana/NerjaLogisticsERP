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
  TextField,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { usePostApiDailyOrdersByIdRejectMutation } from '../../api/dailyOrdersApi';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { buildRejectDailyOrderReasonSchema, type RejectDailyOrderFormValues } from './schemas';

interface RejectDailyOrderDialogProps {
  open: boolean;
  dailyOrderId: string | null;
  onClose: () => void;
}

const defaultValues: RejectDailyOrderFormValues = { reason: '' };

export function RejectDailyOrderDialog({ open, dailyOrderId, onClose }: RejectDailyOrderDialogProps) {
  const { t } = useTranslation('dailyOrders');
  const toast = useToast();
  const [reject, { isLoading: saving }] = usePostApiDailyOrdersByIdRejectMutation();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<RejectDailyOrderFormValues>({
    resolver: zodResolver(buildRejectDailyOrderReasonSchema(t)),
    defaultValues,
  });

  const handleClose = () => {
    reset(defaultValues);
    onClose();
  };

  const onSubmit = async (values: RejectDailyOrderFormValues) => {
    if (!dailyOrderId) return;
    try {
      await reject({ id: dailyOrderId, body: values.reason }).unwrap();
      toast.success(t('team.rejectSuccess'));
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{t('team.rejectDialog.title')}</DialogTitle>
      <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
        <DialogContent>
          <TextField
            {...register('reason')}
            label={t('team.rejectDialog.reason')}
            required
            fullWidth
            multiline
            rows={3}
            sx={{ mt: 1 }}
            error={!!errors.reason}
            helperText={errors.reason?.message}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>{t('common:actions.cancel')}</Button>
          <Button
            type="submit"
            variant="contained"
            color="error"
            disabled={saving}
            startIcon={saving ? <CircularProgress size={18} color="inherit" /> : undefined}
          >
            {t('team.rejectDialog.submit')}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
