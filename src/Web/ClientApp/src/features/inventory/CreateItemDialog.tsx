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
import { usePostApiInventoryItemsMutation } from '../../api/inventoryApi';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { buildInventoryItemSchema, type InventoryItemFormValues } from './schemas';

interface CreateItemDialogProps {
  open: boolean;
  onClose: () => void;
}

const defaultValues: InventoryItemFormValues = { itemName: '', unit: 'pcs', reorderLevel: 0 };

export function CreateItemDialog({ open, onClose }: CreateItemDialogProps) {
  const { t } = useTranslation('inventory');
  const toast = useToast();
  const [createItem, { isLoading: saving }] = usePostApiInventoryItemsMutation();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<InventoryItemFormValues>({
    resolver: zodResolver(buildInventoryItemSchema(t)),
    defaultValues,
  });

  const handleClose = () => {
    reset(defaultValues);
    onClose();
  };

  const onSubmit = async (values: InventoryItemFormValues) => {
    try {
      await createItem({ createInventoryItemCommand: values }).unwrap();
      toast.success(t('createSuccess'));
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{t('createDialog.title')}</DialogTitle>
      <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <TextField
              {...register('itemName')}
              label={t('createDialog.itemName')}
              required
              fullWidth
              error={!!errors.itemName}
              helperText={errors.itemName?.message}
            />
            <TextField
              {...register('unit')}
              label={t('createDialog.unit')}
              required
              fullWidth
              error={!!errors.unit}
              helperText={errors.unit?.message}
            />
            <TextField
              {...register('reorderLevel', { valueAsNumber: true })}
              label={t('createDialog.reorderLevel')}
              type="number"
              required
              fullWidth
              slotProps={{ htmlInput: { min: 0 } }}
              error={!!errors.reorderLevel}
              helperText={errors.reorderLevel?.message}
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
            {t('createDialog.submit')}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
