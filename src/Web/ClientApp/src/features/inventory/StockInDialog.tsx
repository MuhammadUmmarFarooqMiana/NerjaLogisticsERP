import { Controller, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import {
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  MenuItem,
  Stack,
  TextField,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { usePostApiInventoryStockInMutation } from '../../api/inventoryApi';
import { useGetApiSuppliersQuery } from '../../api/suppliersApi';
import type { InventoryItemDto } from '../../api/generated/apiSlice';
import { FormDatePicker } from '../../components/shared/FormDatePicker';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { buildStockInSchema, type StockInFormValues } from './schemas';

interface StockInDialogProps {
  open: boolean;
  item: InventoryItemDto | null;
  onClose: () => void;
}

const defaultValues: StockInFormValues = { supplierId: '', quantity: 0, stockDate: '' };

export function StockInDialog({ open, item, onClose }: StockInDialogProps) {
  const { t } = useTranslation('inventory');
  const toast = useToast();
  const { data: suppliers } = useGetApiSuppliersQuery({}, { skip: !open });
  const [recordStockIn, { isLoading: saving }] = usePostApiInventoryStockInMutation();

  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<StockInFormValues>({
    resolver: zodResolver(buildStockInSchema(t)),
    defaultValues,
  });

  const handleClose = () => {
    reset(defaultValues);
    onClose();
  };

  const onSubmit = async (values: StockInFormValues) => {
    if (!item?.id) return;
    try {
      await recordStockIn({
        recordStockInCommand: { itemId: item.id, ...values },
      }).unwrap();
      toast.success(t('stockInSuccess'));
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{t('stockInDialog.title', { itemName: item?.itemName })}</DialogTitle>
      <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <Controller
              name="supplierId"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  select
                  label={t('stockInDialog.supplier')}
                  required
                  fullWidth
                  error={!!errors.supplierId}
                  helperText={errors.supplierId?.message}
                >
                  {(suppliers ?? []).map((supplier) => (
                    <MenuItem key={supplier.id} value={supplier.id}>
                      {supplier.name}
                    </MenuItem>
                  ))}
                </TextField>
              )}
            />
            <TextField
              {...register('quantity', { valueAsNumber: true })}
              label={t('stockInDialog.quantity')}
              type="number"
              required
              fullWidth
              slotProps={{ htmlInput: { min: 1 } }}
              error={!!errors.quantity}
              helperText={errors.quantity?.message}
            />
            <FormDatePicker
              name="stockDate"
              control={control}
              label={t('stockInDialog.stockDate')}
              required
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
            {t('common:actions.confirm')}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
