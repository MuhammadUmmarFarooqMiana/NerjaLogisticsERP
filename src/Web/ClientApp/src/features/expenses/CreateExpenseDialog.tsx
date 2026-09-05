import { zodResolver } from '@hookform/resolvers/zod';
import { Controller, useForm } from 'react-hook-form';
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
import { usePostApiExpensesMutation } from '../../api/expensesApi';
import { useGetApiPlatformsQuery } from '../../api/generated/apiSlice';
import { FormDatePicker } from '../../components/shared/FormDatePicker';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { EXPENSE_CATEGORIES } from './expenseCategories';
import { buildExpenseSchema, type ExpenseFormValues } from './schemas';

interface CreateExpenseDialogProps {
  open: boolean;
  onClose: () => void;
}

export function CreateExpenseDialog({ open, onClose }: CreateExpenseDialogProps) {
  const { t } = useTranslation('expenses');
  const toast = useToast();
  const { data: platforms } = useGetApiPlatformsQuery(undefined, { skip: !open });
  const [createExpense, { isLoading: saving }] = usePostApiExpensesMutation();

  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ExpenseFormValues>({
    resolver: zodResolver(buildExpenseSchema(t)),
    defaultValues: { category: undefined, amount: 0, expenseDate: '', description: '', platformId: '' },
  });

  const handleClose = () => {
    reset({ category: undefined, amount: 0, expenseDate: '', description: '', platformId: '' });
    onClose();
  };

  const onSubmit = async (values: ExpenseFormValues) => {
    try {
      await createExpense({
        createExpenseCommand: {
          category: values.category,
          amount: values.amount,
          expenseDate: values.expenseDate,
          description: values.description || null,
          platformId: values.platformId || null,
        },
      }).unwrap();
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
            <Controller
              name="category"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  value={field.value ?? ''}
                  onChange={(event) => field.onChange(Number(event.target.value))}
                  select
                  label={t('createDialog.category')}
                  required
                  fullWidth
                  error={!!errors.category}
                  helperText={errors.category?.message}
                >
                  {EXPENSE_CATEGORIES.map((category) => (
                    <MenuItem key={category.value} value={category.value}>
                      {t(`categories.${category.key}`)}
                    </MenuItem>
                  ))}
                </TextField>
              )}
            />
            <TextField
              {...register('amount', { valueAsNumber: true })}
              label={t('createDialog.amount')}
              type="number"
              required
              fullWidth
              slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
              error={!!errors.amount}
              helperText={errors.amount?.message}
            />
            <FormDatePicker
              name="expenseDate"
              control={control}
              label={t('createDialog.expenseDate')}
              required
            />
            <Controller
              name="platformId"
              control={control}
              render={({ field }) => (
                <TextField {...field} select label={t('createDialog.platform')} fullWidth>
                  <MenuItem value="">{t('createDialog.noPlatform')}</MenuItem>
                  {(platforms ?? []).map((platform) => (
                    <MenuItem key={platform.id} value={platform.id}>
                      {platform.name}
                    </MenuItem>
                  ))}
                </TextField>
              )}
            />
            <TextField
              {...register('description')}
              label={t('createDialog.description')}
              fullWidth
              multiline
              rows={2}
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
