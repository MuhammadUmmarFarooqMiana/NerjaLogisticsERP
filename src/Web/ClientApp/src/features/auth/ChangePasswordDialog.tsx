import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Box, Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogTitle, Stack } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { usePostApiAuthChangePasswordMutation } from '../../api/generated/apiSlice';
import { useToast } from '../../components/feedback/ToastContext';
import { PasswordField } from '../../components/shared/PasswordField';
import { getApiErrorMessages } from '../../lib/apiError';
import { buildChangePasswordSchema, type ChangePasswordFormValues } from './schemas';

interface ChangePasswordDialogProps {
  open: boolean;
  onClose: () => void;
}

export function ChangePasswordDialog({ open, onClose }: ChangePasswordDialogProps) {
  const { t } = useTranslation('auth');
  const toast = useToast();
  const [changePassword, { isLoading }] = usePostApiAuthChangePasswordMutation();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ChangePasswordFormValues>({ resolver: zodResolver(buildChangePasswordSchema(t)) });

  const handleClose = () => {
    reset();
    onClose();
  };

  const onSubmit = async (values: ChangePasswordFormValues) => {
    try {
      await changePassword({
        changePasswordCommand: {
          currentPassword: values.currentPassword,
          newPassword: values.newPassword,
          confirmNewPassword: values.confirmNewPassword,
        },
      }).unwrap();
      toast.success(t('changePassword.success'));
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('changePassword.genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="xs" fullWidth>
      <DialogTitle>{t('changePassword.title')}</DialogTitle>
      <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 0.5 }}>
            <PasswordField
              {...register('currentPassword')}
              label={t('changePassword.currentPassword')}
              required
              fullWidth
              autoComplete="current-password"
              error={!!errors.currentPassword}
              helperText={errors.currentPassword?.message}
            />
            <PasswordField
              {...register('newPassword')}
              label={t('changePassword.newPassword')}
              required
              fullWidth
              autoComplete="new-password"
              error={!!errors.newPassword}
              helperText={errors.newPassword?.message}
            />
            <PasswordField
              {...register('confirmNewPassword')}
              label={t('changePassword.confirmNewPassword')}
              required
              fullWidth
              autoComplete="new-password"
              error={!!errors.confirmNewPassword}
              helperText={errors.confirmNewPassword?.message}
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>{t('common:actions.cancel')}</Button>
          <Button
            type="submit"
            variant="contained"
            disabled={isLoading}
            startIcon={isLoading ? <CircularProgress size={18} color="inherit" /> : undefined}
          >
            {t('changePassword.submit')}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
