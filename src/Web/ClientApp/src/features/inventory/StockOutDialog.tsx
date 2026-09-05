import { Controller, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import {
  Alert,
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
import { usePostApiInventoryStockOutMutation } from '../../api/inventoryApi';
import { useGetApiEmployeesQuery } from '../../api/employeesApi';
import { useGetApiMechanicsQuery } from '../../api/mechanicsApi';
import type { InventoryItemDto } from '../../api/generated/apiSlice';
import { FormDatePicker } from '../../components/shared/FormDatePicker';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { employeeOptionLabel } from '../../lib/employeeDisplay';
import { buildStockOutSchema, type StockOutFormValues } from './schemas';

interface StockOutDialogProps {
  open: boolean;
  item: InventoryItemDto | null;
  onClose: () => void;
}

const defaultValues: StockOutFormValues = { mechanicId: '', employeeId: '', quantity: 0, stockDate: '' };

export function StockOutDialog({ open, item, onClose }: StockOutDialogProps) {
  const { t } = useTranslation('inventory');
  const toast = useToast();
  const { data: employees } = useGetApiEmployeesQuery({ status: 'Active' }, { skip: !open });
  const { data: mechanics } = useGetApiMechanicsQuery({}, { skip: !open });
  const [recordStockOut, { isLoading: saving }] = usePostApiInventoryStockOutMutation();

  const {
    control,
    register,
    handleSubmit,
    reset,
    watch,
    formState: { errors },
  } = useForm<StockOutFormValues>({
    resolver: zodResolver(buildStockOutSchema(t)),
    defaultValues,
  });

  // The rest of the form only makes sense once we know who's taking the
  // parts — lock everything else until a mechanic is picked.
  const mechanicId = watch('mechanicId');
  const mechanicPicked = !!mechanicId;

  const handleClose = () => {
    reset(defaultValues);
    onClose();
  };

  const onSubmit = async (values: StockOutFormValues) => {
    if (!item?.id) return;
    try {
      await recordStockOut({
        recordStockOutCommand: { itemId: item.id, ...values },
      }).unwrap();
      toast.success(t('stockOutSuccess'));
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{t('stockOutDialog.title', { itemName: item?.itemName })}</DialogTitle>
      <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <Controller
              name="mechanicId"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  select
                  label={t('stockOutDialog.mechanic')}
                  required
                  fullWidth
                  error={!!errors.mechanicId}
                  helperText={errors.mechanicId?.message}
                >
                  {(mechanics ?? []).map((mechanic) => (
                    <MenuItem key={mechanic.id} value={mechanic.id}>
                      {mechanic.name}
                      {mechanic.specialty ? ` — ${mechanic.specialty}` : ''}
                    </MenuItem>
                  ))}
                </TextField>
              )}
            />

            {!mechanicPicked && <Alert severity="info">{t('stockOutDialog.selectMechanicFirst')}</Alert>}

            <Controller
              name="employeeId"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  select
                  label={t('stockOutDialog.employee')}
                  required
                  fullWidth
                  disabled={!mechanicPicked}
                  error={!!errors.employeeId}
                  helperText={errors.employeeId?.message}
                >
                  {(employees ?? []).map((employee) => (
                    <MenuItem key={employee.id} value={employee.id}>
                      {employeeOptionLabel(employee)}
                    </MenuItem>
                  ))}
                </TextField>
              )}
            />
            <TextField
              {...register('quantity', { valueAsNumber: true })}
              label={t('stockOutDialog.quantity')}
              type="number"
              required
              fullWidth
              disabled={!mechanicPicked}
              slotProps={{ htmlInput: { min: 1, max: item?.currentStock != null ? Number(item.currentStock) : undefined } }}
              error={!!errors.quantity}
              helperText={errors.quantity?.message}
            />
            <FormDatePicker
              name="stockDate"
              control={control}
              label={t('stockOutDialog.stockDate')}
              required
              disabled={!mechanicPicked}
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>{t('common:actions.cancel')}</Button>
          <Button
            type="submit"
            variant="contained"
            disabled={saving || !mechanicPicked}
            startIcon={saving ? <CircularProgress size={18} color="inherit" /> : undefined}
          >
            {t('common:actions.confirm')}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
