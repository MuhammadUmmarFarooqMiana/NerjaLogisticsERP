import { useEffect } from 'react';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import {
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Stack,
  TextField,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { usePostApiSuppliersMutation, usePutApiSuppliersByIdMutation } from '../../api/suppliersApi';
import type { SupplierDto } from '../../api/generated/apiSlice';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { buildSupplierSchema, type SupplierFormValues } from './schemas';

interface SupplierDialogProps {
  open: boolean;
  supplier: SupplierDto | null;
  onClose: () => void;
}

const emptyValues: SupplierFormValues = { name: '', email: '', phone: '', address: '' };

export function SupplierDialog({ open, supplier, onClose }: SupplierDialogProps) {
  const { t } = useTranslation('suppliers');
  const toast = useToast();
  const [createSupplier, { isLoading: creating }] = usePostApiSuppliersMutation();
  const [updateSupplier, { isLoading: updating }] = usePutApiSuppliersByIdMutation();
  const saving = creating || updating;
  const isEdit = !!supplier;

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<SupplierFormValues>({
    resolver: zodResolver(buildSupplierSchema(t)),
    defaultValues: emptyValues,
  });

  useEffect(() => {
    if (open) {
      reset(
        supplier
          ? {
              name: supplier.name,
              email: supplier.email ?? '',
              phone: supplier.phone ?? '',
              address: supplier.address ?? '',
            }
          : emptyValues
      );
    }
  }, [open, supplier, reset]);

  const handleClose = () => {
    reset(emptyValues);
    onClose();
  };

  const onSubmit = async (values: SupplierFormValues) => {
    const payload = {
      name: values.name,
      email: values.email || null,
      phone: values.phone || null,
      address: values.address || null,
    };
    try {
      if (isEdit && supplier?.id) {
        await updateSupplier({ id: supplier.id, updateSupplierCommand: { id: supplier.id, ...payload } }).unwrap();
        toast.success(t('updateSuccess'));
      } else {
        await createSupplier({ createSupplierCommand: payload }).unwrap();
        toast.success(t('createSuccess'));
      }
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{isEdit ? t('editDialog.title') : t('createDialog.title')}</DialogTitle>
      <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              {...register('name')}
              label={t('fields.name')}
              required
              fullWidth
              error={!!errors.name}
              helperText={errors.name?.message}
            />
            <TextField
              {...register('email')}
              label={t('fields.email')}
              fullWidth
              error={!!errors.email}
              helperText={errors.email?.message}
            />
            <TextField {...register('phone')} label={t('fields.phone')} fullWidth />
            <TextField {...register('address')} label={t('fields.address')} fullWidth multiline rows={2} />
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
            {t('common:actions.confirm')}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
