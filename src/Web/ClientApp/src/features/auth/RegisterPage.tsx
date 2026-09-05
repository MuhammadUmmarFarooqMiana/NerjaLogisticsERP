import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Link as RouterLink, useNavigate } from 'react-router-dom';
import {
  Box,
  Button,
  Checkbox,
  CircularProgress,
  FormControlLabel,
  Link,
  Paper,
  TextField,
  Typography,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { usePostApiAuthLoginMutation, usePostApiAuthRegisterMutation } from '../../api/generated/apiSlice';
import { useAppDispatch } from '../../app/hooks';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { storeBrowserPasswordCredential } from '../../lib/browserCredentials';
import { PasswordField } from '../../components/shared/PasswordField';
import { AuthLayout } from './AuthLayout';
import { credentialsReceived } from './authSlice';
import { buildRegisterSchema, type RegisterFormValues } from './schemas';

export default function RegisterPage() {
  const { t } = useTranslation('auth');
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const toast = useToast();
  const [registerEmployee, { isLoading: registering }] = usePostApiAuthRegisterMutation();
  const [login, { isLoading: loggingIn }] = usePostApiAuthLoginMutation();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterFormValues>({
    resolver: zodResolver(buildRegisterSchema(t)),
    defaultValues: { fullName: '', email: '', password: '', phoneNumber: '', hasWhatsApp: false },
  });

  const onSubmit = async (values: RegisterFormValues) => {
    try {
      await registerEmployee({ registerEmployeeCommand: values }).unwrap();
    } catch (err) {
      const messages = getApiErrorMessages(err, t('register.genericError'));
      toast.error(messages.join('\n'));
      return;
    }

    // Registration succeeded — sign them straight in instead of bouncing to the
    // login form, and let the browser offer to save the password (a JS-driven
    // login doesn't trigger that prompt on its own, unlike a real form submit).
    try {
      const loginResult = await login({ loginCommand: { email: values.email, password: values.password } }).unwrap();
      dispatch(credentialsReceived(loginResult));
      void storeBrowserPasswordCredential({ id: values.email, password: values.password, name: values.fullName });
      toast.success(t('register.welcomeMessage', { name: values.fullName }));
      // Always land on Dashboard — RequireActiveProfile bounces Incomplete/PendingReview
      // users to /profile automatically, so this doesn't need its own special case.
      navigate('/dashboard', { replace: true });
    } catch {
      // Registered fine but auto-login failed for some reason — fall back to
      // the manual login flow rather than losing the successful registration.
      navigate('/login', { replace: true, state: { registered: true } });
    }
  };

  const isLoading = registering || loggingIn;

  return (
    <AuthLayout>
      <Paper elevation={0} sx={{ p: { xs: 3, sm: 4 }, border: '1px solid', borderColor: 'divider', borderRadius: '24px' }}>
        <Typography variant="h5" fontWeight={700} gutterBottom>
          {t('register.title')}
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
          {t('register.subtitle')}
        </Typography>

        <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
          <TextField
            {...register('fullName')}
            label={t('register.fullName')}
            required
            fullWidth
            margin="normal"
            autoComplete="name"
            error={!!errors.fullName}
            helperText={errors.fullName?.message}
          />
          <TextField
            {...register('email')}
            label={t('register.email')}
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
            label={t('register.password')}
            required
            fullWidth
            margin="normal"
            autoComplete="new-password"
            error={!!errors.password}
            helperText={errors.password?.message ?? t('validation.passwordComplexity')}
          />
          <TextField
            {...register('phoneNumber')}
            label={t('register.phoneNumber')}
            required
            fullWidth
            margin="normal"
            autoComplete="tel"
            placeholder="+9665XXXXXXXX"
            error={!!errors.phoneNumber}
            helperText={errors.phoneNumber?.message}
          />
          <FormControlLabel
            control={<Checkbox {...register('hasWhatsApp')} />}
            label={t('register.hasWhatsApp')}
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
            {t('register.submit')}
          </Button>
        </Box>

        <Typography variant="body2" sx={{ mt: 3, textAlign: 'center' }}>
          {t('register.haveAccount')} <Link component={RouterLink} to="/login">{t('register.loginLink')}</Link>
        </Typography>
      </Paper>
    </AuthLayout>
  );
}
