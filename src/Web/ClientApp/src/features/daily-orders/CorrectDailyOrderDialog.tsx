import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import {
  Alert,
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
import { usePostApiDailyOrdersByIdCorrectMutation } from '../../api/dailyOrdersApi';
import type { DailyOrderListItemDto } from '../../api/generated/apiSlice';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { buildCorrectDailyOrderSchema, type CorrectDailyOrderFormValues } from './schemas';

interface CorrectDailyOrderDialogProps {
  open: boolean;
  order: DailyOrderListItemDto | null;
  onClose: () => void;
}

export function CorrectDailyOrderDialog({ open, order, onClose }: CorrectDailyOrderDialogProps) {
  const { t } = useTranslation('dailyOrders');
  const toast = useToast();
  const [correct, { isLoading: saving }] = usePostApiDailyOrdersByIdCorrectMutation();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CorrectDailyOrderFormValues>({
    resolver: zodResolver(buildCorrectDailyOrderSchema(t)),
    defaultValues: { completedOrders: 0, reason: '' },
  });

  // The dialog mounts once and is reused across rows, so the form needs to
  // resync to whichever order was just clicked rather than keeping the
  // previous row's stale count.
  useEffect(() => {
    if (order) reset({ completedOrders: Number(order.completedOrders), reason: '' });
  }, [order, reset]);

  const handleClose = () => {
    onClose();
  };

  const onSubmit = async (values: CorrectDailyOrderFormValues) => {
    if (!order?.id) return;
    try {
      await correct({
        id: order.id,
        correctDailyOrderCommand: { completedOrders: values.completedOrders, reason: values.reason },
      }).unwrap();
      toast.success(t('team.correctSuccess'));
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{t('team.correctDialog.title')}</DialogTitle>
      <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
        <DialogContent>
          {order?.reviewNote && (
            <Alert severity="warning" sx={{ mb: 2 }}>
              {t('team.correctDialog.previousReason', { reason: order.reviewNote })}
            </Alert>
          )}
          <TextField
            {...register('completedOrders', { valueAsNumber: true })}
            label={t('team.correctDialog.completedOrders')}
            type="number"
            required
            fullWidth
            slotProps={{ htmlInput: { min: 0 } }}
            error={!!errors.completedOrders}
            helperText={errors.completedOrders?.message}
          />
          <TextField
            {...register('reason')}
            label={t('team.correctDialog.reason')}
            required
            fullWidth
            multiline
            rows={3}
            sx={{ mt: 2 }}
            error={!!errors.reason}
            helperText={errors.reason?.message}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>{t('common:actions.cancel')}</Button>
          <Button
            type="submit"
            variant="contained"
            disabled={saving}
            startIcon={saving ? <CircularProgress size={18} color="inherit" /> : undefined}
          >
            {t('team.correctDialog.submit')}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
