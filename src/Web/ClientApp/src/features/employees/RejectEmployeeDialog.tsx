import { useState } from 'react';
import { Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogTitle, TextField } from '@mui/material';
import { useTranslation } from 'react-i18next';

interface RejectEmployeeDialogProps {
  open: boolean;
  loading?: boolean;
  onCancel: () => void;
  onConfirm: (reason: string) => void;
}

export function RejectEmployeeDialog({ open, loading = false, onCancel, onConfirm }: RejectEmployeeDialogProps) {
  const { t } = useTranslation('employees');
  const [reason, setReason] = useState('');

  const handleConfirm = () => {
    if (!reason.trim()) return;
    onConfirm(reason.trim());
  };

  return (
    <Dialog open={open} onClose={onCancel} fullWidth maxWidth="sm">
      <DialogTitle>{t('detail.rejectDialogTitle')}</DialogTitle>
      <DialogContent>
        <TextField
          autoFocus
          required
          fullWidth
          multiline
          minRows={3}
          label={t('detail.rejectReasonLabel')}
          value={reason}
          onChange={(event) => setReason(event.target.value)}
          sx={{ mt: 1 }}
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={onCancel} disabled={loading}>
          {t('common:actions.cancel')}
        </Button>
        <Button
          onClick={handleConfirm}
          color="error"
          variant="contained"
          disabled={loading || !reason.trim()}
          startIcon={loading ? <CircularProgress size={18} color="inherit" /> : undefined}
        >
          {t('detail.rejectSubmit')}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
