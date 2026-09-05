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
import { usePostApiLeaveRequestsByIdRejectMutation } from '../../api/leaveRequestsApi';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { buildRejectReasonSchema, type RejectReasonFormValues } from './schemas';

interface RejectLeaveRequestDialogProps {
  open: boolean;
  leaveRequestId: string | null;
  onClose: () => void;
}

const defaultValues: RejectReasonFormValues = { reason: '' };

export function RejectLeaveRequestDialog({ open, leaveRequestId, onClose }: RejectLeaveRequestDialogProps) {
  const { t } = useTranslation('leaveRequests');
  const toast = useToast();
  const [reject, { isLoading: saving }] = usePostApiLeaveRequestsByIdRejectMutation();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<RejectReasonFormValues>({
    resolver: zodResolver(buildRejectReasonSchema(t)),
    defaultValues,
  });

  const handleClose = () => {
    reset(defaultValues);
    onClose();
  };

  const onSubmit = async (values: RejectReasonFormValues) => {
    if (!leaveRequestId) return;
    try {
      await reject({ id: leaveRequestId, body: values.reason }).unwrap();
      toast.success(t('rejectSuccess'));
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{t('rejectDialog.title')}</DialogTitle>
      <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
        <DialogContent>
          <TextField
            {...register('reason')}
            label={t('rejectDialog.reason')}
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
            {t('rejectDialog.submit')}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
