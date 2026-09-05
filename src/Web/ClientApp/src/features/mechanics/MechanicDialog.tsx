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
import { usePostApiMechanicsMutation, usePutApiMechanicsByIdMutation } from '../../api/mechanicsApi';
import type { MechanicDto } from '../../api/generated/apiSlice';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { buildMechanicSchema, type MechanicFormValues } from './schemas';

interface MechanicDialogProps {
  open: boolean;
  mechanic: MechanicDto | null;
  onClose: () => void;
}

const emptyValues: MechanicFormValues = { name: '', phone: '', email: '', specialty: '', address: '' };

export function MechanicDialog({ open, mechanic, onClose }: MechanicDialogProps) {
  const { t } = useTranslation('mechanics');
  const toast = useToast();
  const [createMechanic, { isLoading: creating }] = usePostApiMechanicsMutation();
  const [updateMechanic, { isLoading: updating }] = usePutApiMechanicsByIdMutation();
  const saving = creating || updating;
  const isEdit = !!mechanic;

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<MechanicFormValues>({
    resolver: zodResolver(buildMechanicSchema(t)),
    defaultValues: emptyValues,
  });

  useEffect(() => {
    if (open) {
      reset(
        mechanic
          ? {
              name: mechanic.name,
              phone: mechanic.phone ?? '',
              email: mechanic.email ?? '',
              specialty: mechanic.specialty ?? '',
              address: mechanic.address ?? '',
            }
          : emptyValues
      );
    }
  }, [open, mechanic, reset]);

  const handleClose = () => {
    reset(emptyValues);
    onClose();
  };

  const onSubmit = async (values: MechanicFormValues) => {
    const payload = {
      name: values.name,
      phone: values.phone || null,
      email: values.email || null,
      specialty: values.specialty || null,
      address: values.address || null,
    };
    try {
      if (isEdit && mechanic?.id) {
        await updateMechanic({ id: mechanic.id, updateMechanicCommand: { id: mechanic.id, ...payload } }).unwrap();
        toast.success(t('updateSuccess'));
      } else {
        await createMechanic({ createMechanicCommand: payload }).unwrap();
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
            <TextField {...register('phone')} label={t('fields.phone')} fullWidth />
            <TextField
              {...register('email')}
              label={t('fields.email')}
              fullWidth
              error={!!errors.email}
              helperText={errors.email?.message}
            />
            <TextField
              {...register('specialty')}
              label={t('fields.specialty')}
              fullWidth
              helperText={t('fields.specialtyHint')}
            />
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
