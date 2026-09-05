import { zodResolver } from '@hookform/resolvers/zod';
import { Controller, useFieldArray, useForm } from 'react-hook-form';
import AddIcon from '@mui/icons-material/Add';
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutlineOutlined';
import {
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Divider,
  IconButton,
  MenuItem,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { usePostApiSalaryFormulasMutation } from '../../api/salaryFormulasApi';
import { useGetApiPlatformsQuery } from '../../api/generated/apiSlice';
import { FormDatePicker } from '../../components/shared/FormDatePicker';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { FIXED_MONTHLY, FORMULA_TYPES, TIERED_PER_ORDER, TIER_RATE_TYPES } from './salaryFormulaEnums';
import { buildSalaryFormulaSchema, type SalaryFormulaFormValues } from './schemas';

interface CreateSalaryFormulaDialogProps {
  open: boolean;
  onClose: () => void;
}

const emptyTier = { minOrders: 0, maxOrders: undefined, rateType: 0, rate: 0 };

export function CreateSalaryFormulaDialog({ open, onClose }: CreateSalaryFormulaDialogProps) {
  const { t } = useTranslation('salaryFormulas');
  const toast = useToast();
  const { data: platforms } = useGetApiPlatformsQuery(undefined, { skip: !open });
  const [createFormula, { isLoading: saving }] = usePostApiSalaryFormulasMutation();

  const {
    control,
    register,
    handleSubmit,
    reset,
    watch,
    formState: { errors },
  } = useForm<SalaryFormulaFormValues>({
    resolver: zodResolver(buildSalaryFormulaSchema(t)),
    defaultValues: {
      platformId: '',
      formulaType: TIERED_PER_ORDER,
      fixedMonthlyAmount: undefined,
      effectiveFrom: '',
      tiers: [emptyTier],
    },
  });

  const { fields, append, remove } = useFieldArray({ control, name: 'tiers' });
  const formulaType = watch('formulaType');

  const handleClose = () => {
    reset({
      platformId: '',
      formulaType: TIERED_PER_ORDER,
      fixedMonthlyAmount: undefined,
      effectiveFrom: '',
      tiers: [emptyTier],
    });
    onClose();
  };

  const onSubmit = async (values: SalaryFormulaFormValues) => {
    try {
      await createFormula({
        createSalaryFormulaCommand: {
          platformId: values.platformId || null,
          formulaType: values.formulaType,
          fixedMonthlyAmount: values.formulaType === FIXED_MONTHLY ? values.fixedMonthlyAmount : null,
          effectiveFrom: values.effectiveFrom,
          tiers:
            values.formulaType === TIERED_PER_ORDER
              ? values.tiers.map((tier) => ({
                  minOrders: tier.minOrders,
                  maxOrders: tier.maxOrders ?? null,
                  rateType: tier.rateType,
                  rate: tier.rate,
                }))
              : [],
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
              name="platformId"
              control={control}
              render={({ field }) => (
                <TextField {...field} select label={t('createDialog.platform')} fullWidth>
                  <MenuItem value="">{t('createDialog.genericDefault')}</MenuItem>
                  {(platforms ?? []).map((platform) => (
                    <MenuItem key={platform.id} value={platform.id}>
                      {platform.name}
                    </MenuItem>
                  ))}
                </TextField>
              )}
            />
            <Controller
              name="formulaType"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  onChange={(event) => field.onChange(Number(event.target.value))}
                  select
                  label={t('createDialog.formulaType')}
                  required
                  fullWidth
                >
                  {FORMULA_TYPES.map((option) => (
                    <MenuItem key={option.value} value={option.value}>
                      {t(`formulaTypes.${option.key}`)}
                    </MenuItem>
                  ))}
                </TextField>
              )}
            />
            <FormDatePicker
              name="effectiveFrom"
              control={control}
              label={t('createDialog.effectiveFrom')}
              required
            />

            {formulaType === FIXED_MONTHLY && (
              <TextField
                {...register('fixedMonthlyAmount', { valueAsNumber: true })}
                label={t('createDialog.fixedMonthlyAmount')}
                type="number"
                required
                fullWidth
                slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
                error={!!errors.fixedMonthlyAmount}
                helperText={errors.fixedMonthlyAmount?.message}
              />
            )}

            {formulaType === TIERED_PER_ORDER && (
              <Stack spacing={1.5}>
                <Divider />
                <Typography variant="subtitle2">{t('createDialog.tiers')}</Typography>
                {typeof errors.tiers?.message === 'string' && (
                  <Typography variant="caption" color="error">
                    {errors.tiers.message}
                  </Typography>
                )}
                {fields.map((field, index) => (
                  <Stack key={field.id} direction="row" spacing={1} sx={{ alignItems: 'flex-start' }}>
                    <TextField
                      {...register(`tiers.${index}.minOrders`, { valueAsNumber: true })}
                      label={t('createDialog.minOrders')}
                      type="number"
                      size="small"
                      slotProps={{ htmlInput: { min: 0 } }}
                      error={!!errors.tiers?.[index]?.minOrders}
                    />
                    <TextField
                      {...register(`tiers.${index}.maxOrders`, { valueAsNumber: true })}
                      label={t('createDialog.maxOrders')}
                      type="number"
                      size="small"
                      slotProps={{ htmlInput: { min: 0 } }}
                      error={!!errors.tiers?.[index]?.maxOrders}
                      helperText={errors.tiers?.[index]?.maxOrders?.message}
                    />
                    <Controller
                      name={`tiers.${index}.rateType`}
                      control={control}
                      render={({ field: rateTypeField }) => (
                        <TextField
                          {...rateTypeField}
                          onChange={(event) => rateTypeField.onChange(Number(event.target.value))}
                          select
                          label={t('createDialog.rateType')}
                          size="small"
                          sx={{ minWidth: 140 }}
                        >
                          {TIER_RATE_TYPES.map((option) => (
                            <MenuItem key={option.value} value={option.value}>
                              {t(`rateTypes.${option.key}`)}
                            </MenuItem>
                          ))}
                        </TextField>
                      )}
                    />
                    <TextField
                      {...register(`tiers.${index}.rate`, { valueAsNumber: true })}
                      label={t('createDialog.rate')}
                      type="number"
                      size="small"
                      slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
                      error={!!errors.tiers?.[index]?.rate}
                    />
                    <IconButton
                      onClick={() => remove(index)}
                      disabled={fields.length === 1}
                      aria-label={t('createDialog.removeTier')}
                      size="small"
                    >
                      <DeleteOutlineIcon fontSize="small" />
                    </IconButton>
                  </Stack>
                ))}
                <Button
                  startIcon={<AddIcon />}
                  onClick={() => append(emptyTier)}
                  size="small"
                  sx={{ alignSelf: 'flex-start' }}
                >
                  {t('createDialog.addTier')}
                </Button>
              </Stack>
            )}
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
