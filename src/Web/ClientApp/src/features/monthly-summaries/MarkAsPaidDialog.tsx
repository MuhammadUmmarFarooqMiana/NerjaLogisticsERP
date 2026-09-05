import { useState } from 'react';
import {
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  TextField,
} from '@mui/material';
import { useTranslation } from 'react-i18next';

interface MarkAsPaidDialogProps {
  open: boolean;
  loading: boolean;
  onConfirm: (paymentReference: string) => void;
  onCancel: () => void;
}

export function MarkAsPaidDialog({ open, loading, onConfirm, onCancel }: MarkAsPaidDialogProps) {
  const { t } = useTranslation('monthlySummaries');
  const [paymentReference, setPaymentReference] = useState('');

  const handleClose = () => {
    setPaymentReference('');
    onCancel();
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="xs" fullWidth>
      <DialogTitle>{t('markAsPaidDialog.title')}</DialogTitle>
      <DialogContent>
        <TextField
          label={t('markAsPaidDialog.paymentReference')}
          fullWidth
          value={paymentReference}
          onChange={(event) => setPaymentReference(event.target.value)}
          sx={{ mt: 1 }}
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>{t('common:actions.cancel')}</Button>
        <Button
          variant="contained"
          disabled={loading}
          startIcon={loading ? <CircularProgress size={18} color="inherit" /> : undefined}
          onClick={() => onConfirm(paymentReference)}
        >
          {t('markAsPaidDialog.submit')}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
