import { zodResolver } from '@hookform/resolvers/zod';
import { Controller, useForm } from 'react-hook-form';
import {
  Box,
  Button,
  Checkbox,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Divider,
  FormControlLabel,
  FormGroup,
  MenuItem,
  Stack,
  Switch,
  TextField,
  Typography,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { usePostApiEmployeesMutation } from '../../api/employeesApi';
import { useGetApiPlatformsQuery } from '../../api/generated/apiSlice';
import { FormDatePicker } from '../../components/shared/FormDatePicker';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { Roles } from '../../lib/roles';
import { buildCreateEmployeeSchema, type CreateEmployeeFormValues } from './schemas';

interface CreateEmployeeDialogProps {
  open: boolean;
  onClose: () => void;
}

const ALL_ROLES = Object.values(Roles);

const defaultValues: CreateEmployeeFormValues = {
  fullName: '',
  email: '',
  password: '',
  phoneNumber: '',
  hasWhatsApp: true,
  roles: [],
  iqamaNumber: '',
  platformId: '',
  platformIdNumber: '',
  joiningDate: '',
  idExpiryDate: '',
  iqamaExpiryDate: '',
  drivingLicenseExpiryDate: '',
  insuranceExpiryDate: '',
};

export function CreateEmployeeDialog({ open, onClose }: CreateEmployeeDialogProps) {
  const { t } = useTranslation('employees');
  const toast = useToast();
  const { data: platforms } = useGetApiPlatformsQuery(undefined, { skip: !open });
  const [createEmployee, { isLoading: saving }] = usePostApiEmployeesMutation();

  const {
    control,
    register,
    handleSubmit,
    reset,
    watch,
    formState: { errors },
  } = useForm<CreateEmployeeFormValues>({
    resolver: zodResolver(buildCreateEmployeeSchema(t)),
    defaultValues,
  });

  const platformId = watch('platformId');

  const handleClose = () => {
    reset(defaultValues);
    onClose();
  };

  const onSubmit = async (values: CreateEmployeeFormValues) => {
    try {
      await createEmployee({
        adminCreateEmployeeCommand: {
          fullName: values.fullName,
          email: values.email,
          password: values.password,
          phoneNumber: values.phoneNumber,
          hasWhatsApp: values.hasWhatsApp,
          roles: values.roles,
          iqamaNumber: values.iqamaNumber,
          platformId: values.platformId || null,
          platformIdNumber: values.platformId ? values.platformIdNumber || null : null,
          joiningDate: values.joiningDate || null,
          idExpiryDate: values.idExpiryDate || null,
          iqamaExpiryDate: values.iqamaExpiryDate || null,
          drivingLicenseExpiryDate: values.drivingLicenseExpiryDate || null,
          insuranceExpiryDate: values.insuranceExpiryDate || null,
        },
      }).unwrap();
      toast.success(t('admin.createSuccess'));
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('detail.genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{t('admin.createDialog.title')}</DialogTitle>
      <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              {...register('fullName')}
              label={t('admin.createDialog.fullName')}
              required
              fullWidth
              error={!!errors.fullName}
              helperText={errors.fullName?.message}
            />
            <TextField
              {...register('email')}
              label={t('admin.createDialog.email')}
              type="email"
              required
              fullWidth
              // Browsers match this field + the password field below against the
              // Administrator's own saved login for this site and silently
              // autofill both — "off" alone doesn't reliably stop Chrome once it
              // recognizes the email+password pair, but combined with the
              // password field's autoComplete="new-password" below it does.
              autoComplete="off"
              error={!!errors.email}
              helperText={errors.email?.message}
            />
            <TextField
              {...register('password')}
              label={t('admin.createDialog.password')}
              type="password"
              required
              fullWidth
              // "new-password" tells the browser this sets a password for a new
              // account rather than filling in a saved login — the standard fix
              // for admin "create user" forms getting autofilled with the
              // signed-in user's own credentials.
              autoComplete="new-password"
              error={!!errors.password}
              helperText={errors.password?.message}
            />
            <TextField
              {...register('phoneNumber')}
              label={t('admin.createDialog.phoneNumber')}
              required
              fullWidth
              error={!!errors.phoneNumber}
              helperText={errors.phoneNumber?.message}
            />
            <Controller
              name="hasWhatsApp"
              control={control}
              render={({ field }) => (
                <FormControlLabel
                  control={<Switch checked={field.value} onChange={(e) => field.onChange(e.target.checked)} />}
                  label={t('detail.fields.hasWhatsApp')}
                />
              )}
            />

            <Divider />
            <Typography variant="subtitle2">{t('admin.createDialog.roles')}</Typography>
            {typeof errors.roles?.message === 'string' && (
              <Typography variant="caption" color="error">
                {errors.roles.message}
              </Typography>
            )}
            <Controller
              name="roles"
              control={control}
              render={({ field }) => (
                <FormGroup row>
                  {ALL_ROLES.map((role) => (
                    <FormControlLabel
                      key={role}
                      control={
                        <Checkbox
                          checked={field.value.includes(role)}
                          onChange={(event) => {
                            field.onChange(
                              event.target.checked
                                ? [...field.value, role]
                                : field.value.filter((r) => r !== role)
                            );
                          }}
                        />
                      }
                      label={t(`admin.roles.${role}`)}
                    />
                  ))}
                </FormGroup>
              )}
            />

            <Divider />
            <TextField
              {...register('iqamaNumber')}
              label={t('detail.fields.iqamaNumber')}
              required
              fullWidth
              error={!!errors.iqamaNumber}
              helperText={errors.iqamaNumber?.message}
            />
            <Controller
              name="platformId"
              control={control}
              render={({ field }) => (
                <TextField {...field} select label={t('detail.fields.platform')} fullWidth>
                  <MenuItem value="">{t('admin.createDialog.noPlatform')}</MenuItem>
                  {(platforms ?? []).map((platform) => (
                    <MenuItem key={platform.id} value={platform.id}>
                      {platform.name}
                    </MenuItem>
                  ))}
                </TextField>
              )}
            />
            {platformId && (
              <TextField
                {...register('platformIdNumber')}
                label={t('detail.fields.platformIdNumber')}
                fullWidth
              />
            )}
            <FormDatePicker name="joiningDate" control={control} label={t('detail.fields.joiningDate')} />
            <FormDatePicker name="idExpiryDate" control={control} label={t('detail.fields.idExpiryDate')} />
            <FormDatePicker name="iqamaExpiryDate" control={control} label={t('detail.fields.iqamaExpiryDate')} />
            <FormDatePicker
              name="drivingLicenseExpiryDate"
              control={control}
              label={t('detail.fields.drivingLicenseExpiryDate')}
            />
            <FormDatePicker
              name="insuranceExpiryDate"
              control={control}
              label={t('detail.fields.insuranceExpiryDate')}
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
            {t('admin.createDialog.submit')}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
