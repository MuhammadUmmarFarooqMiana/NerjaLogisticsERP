import { useEffect } from 'react';
import { zodResolver } from '@hookform/resolvers/zod';
import { Controller, useForm } from 'react-hook-form';
import { Box, Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogTitle, MenuItem, Stack, TextField } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { usePutApiEmployeesByIdMutation } from '../../api/employeesApi';
import { useGetApiPlatformsQuery } from '../../api/generated/apiSlice';
import type { EmployeeDetailDto } from '../../api/generated/apiSlice';
import { FormDatePicker } from '../../components/shared/FormDatePicker';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { buildEditEmployeeSchema, type EditEmployeeFormValues } from './schemas';

interface EditEmployeeDialogProps {
  open: boolean;
  employee: EmployeeDetailDto | null;
  onClose: () => void;
}

const defaultValues: EditEmployeeFormValues = {
  fullName: '',
  iqamaNumber: '',
  platformId: '',
  idExpiryDate: '',
  iqamaExpiryDate: '',
  drivingLicenseExpiryDate: '',
  insuranceExpiryDate: '',
};

export function EditEmployeeDialog({ open, employee, onClose }: EditEmployeeDialogProps) {
  const { t } = useTranslation('employees');
  const toast = useToast();
  const { data: platforms } = useGetApiPlatformsQuery(undefined, { skip: !open });
  const [updateEmployee, { isLoading: saving }] = usePutApiEmployeesByIdMutation();

  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<EditEmployeeFormValues>({
    resolver: zodResolver(buildEditEmployeeSchema(t)),
    defaultValues,
  });

  useEffect(() => {
    if (!employee) return;
    reset({
      fullName: employee.fullName ?? '',
      iqamaNumber: employee.iqamaNumber ?? '',
      platformId: employee.platformId ?? '',
      idExpiryDate: employee.idExpiryDate ?? '',
      iqamaExpiryDate: employee.iqamaExpiryDate ?? '',
      drivingLicenseExpiryDate: employee.drivingLicenseExpiryDate ?? '',
      insuranceExpiryDate: employee.insuranceExpiryDate ?? '',
    });
  }, [employee, reset]);

  const handleClose = () => {
    reset(defaultValues);
    onClose();
  };

  const onSubmit = async (values: EditEmployeeFormValues) => {
    if (!employee?.id) return;
    try {
      await updateEmployee({
        id: employee.id,
        adminUpdateEmployeeCommand: {
          id: employee.id,
          fullName: values.fullName,
          iqamaNumber: values.iqamaNumber || null,
          platformId: values.platformId || null,
          idExpiryDate: values.idExpiryDate || null,
          iqamaExpiryDate: values.iqamaExpiryDate || null,
          drivingLicenseExpiryDate: values.drivingLicenseExpiryDate || null,
          insuranceExpiryDate: values.insuranceExpiryDate || null,
        },
      }).unwrap();
      toast.success(t('admin.updateSuccess'));
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('detail.genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{t('admin.editDialog.title')}</DialogTitle>
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
            <TextField {...register('iqamaNumber')} label={t('detail.fields.iqamaNumber')} fullWidth />
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
            {t('admin.editDialog.submit')}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
