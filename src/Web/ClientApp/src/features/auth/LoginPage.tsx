import { useEffect } from 'react';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Link as RouterLink, useLocation, useNavigate } from 'react-router-dom';
import { Box, Button, CircularProgress, Link, Paper, TextField, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { usePostApiAuthLoginMutation } from '../../api/generated/apiSlice';
import { useAppDispatch } from '../../app/hooks';
import { useToast } from '../../components/feedback/ToastContext';
import { PasswordField } from '../../components/shared/PasswordField';
import { AuthLayout } from './AuthLayout';
import { credentialsReceived } from './authSlice';
import { buildLoginSchema, type LoginFormValues } from './schemas';

export default function LoginPage() {
  const { t } = useTranslation('auth');
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const location = useLocation();
  const toast = useToast();
  const [login, { isLoading }] = usePostApiAuthLoginMutation();
  const justRegistered = (location.state as { registered?: boolean } | null)?.registered ?? false;

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormValues>({ resolver: zodResolver(buildLoginSchema(t)) });

  useEffect(() => {
    if (justRegistered) {
      toast.success(t('register.success'));
      navigate(location.pathname, { replace: true, state: null });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [justRegistered]);

  const onSubmit = async (values: LoginFormValues) => {
    try {
      const result = await login({ loginCommand: values }).unwrap();
      dispatch(credentialsReceived(result));
      toast.success(t('login.success'));
      // Always land on Dashboard, regardless of which page redirected here —
      // RequireActiveProfile bounces Incomplete/PendingReview users to
      // /profile automatically, and the rejected-profile notice is shown
      // there (on Dashboard), not gating login itself.
      navigate('/dashboard', { replace: true });
    } catch (err) {
      const status = (err as { status?: number } | undefined)?.status;
      toast.error(status === 403 ? t('login.terminatedError') : t('login.genericError'));
    }
  };

  return (
    <AuthLayout>
      <Paper elevation={0} sx={{ p: { xs: 3, sm: 4 }, border: '1px solid', borderColor: 'divider', borderRadius: '24px' }}>
        <Typography variant="h5" fontWeight={700} gutterBottom>
          {t('login.title')}
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
          {t('login.subtitle')}
        </Typography>

        <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
          <TextField
            {...register('email')}
            label={t('login.email')}
            type="email"
            required
            fullWidth
            margin="normal"
            autoComplete="email"
            error={!!errors.email}
            helperText={errors.email?.message}
          />
          <PasswordField
            {...register('password')}
            label={t('login.password')}
            required
            fullWidth
            margin="normal"
            autoComplete="current-password"
            error={!!errors.password}
            helperText={errors.password?.message}
          />
          <Button
            type="submit"
            variant="contained"
            fullWidth
            size="large"
            sx={{ mt: 3, py: 1.25 }}
            disabled={isLoading}
            startIcon={isLoading ? <CircularProgress size={18} color="inherit" /> : undefined}
          >
            {t('login.submit')}
          </Button>
        </Box>

        <Typography variant="body2" sx={{ mt: 3, textAlign: 'center' }}>
          {t('login.noAccount')} <Link component={RouterLink} to="/register">{t('login.registerLink')}</Link>
        </Typography>
      </Paper>
    </AuthLayout>
  );
}
