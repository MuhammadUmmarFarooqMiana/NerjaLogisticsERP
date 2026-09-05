import { useEffect } from 'react';
import { zodResolver } from '@hookform/resolvers/zod';
import { Controller, useFieldArray, useForm } from 'react-hook-form';
import AddIcon from '@mui/icons-material/Add';
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutlineOutlined';
import {
  Box,
  Button,
  Chip,
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
import { useGetApiSalaryFormulasByIdQuery, usePutApiSalaryFormulasByIdMutation } from '../../api/salaryFormulasApi';
import { useAppSelector } from '../../app/hooks';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { formatDate } from '../../lib/formatDate';
import { Roles } from '../../lib/roles';
import { selectCurrentUser } from '../auth/authSlice';
import { FIXED_MONTHLY, TIER_RATE_TYPES, TIERED_PER_ORDER } from './salaryFormulaEnums';
import { buildSalaryFormulaEditSchema, type SalaryFormulaEditFormValues } from './schemas';

interface SalaryFormulaDetailDialogProps {
  open: boolean;
  formulaId: string | null;
  onClose: () => void;
}

const emptyTier = { minOrders: 0, maxOrders: undefined, rateType: 0, rate: 0 };
const defaultValues: SalaryFormulaEditFormValues = { fixedMonthlyAmount: undefined, tiers: [emptyTier] };

// Details + edit live in one dialog rather than a separate view/edit pair: the read-only fields
// (Platform, Formula Type, Effective From, Status) never change here — only the terms below them
// (amount or tier rates) become editable inputs, gated on role and on the formula still being
// active. See Application/Salaries/Commands/UpdateSalaryFormula for why those four stay read-only.
export function SalaryFormulaDetailDialog({ open, formulaId, onClose }: SalaryFormulaDetailDialogProps) {
  const { t, i18n } = useTranslation('salaryFormulas');
  const toast = useToast();
  const user = useAppSelector(selectCurrentUser);
  const canEdit = !!user?.roles.some((role) => role === Roles.Administrator);

  const {
    data: formula,
    isLoading,
    error,
  } = useGetApiSalaryFormulasByIdQuery({ id: formulaId ?? '' }, { skip: !open || !formulaId });
  const [updateFormula, { isLoading: saving }] = usePutApiSalaryFormulasByIdMutation();

  const isActive = formula?.effectiveTo == null;
  const formulaTypeValue = formula?.formulaType === 'FixedMonthly' ? FIXED_MONTHLY : TIERED_PER_ORDER;
  const isEditable = canEdit && !!formula && isActive;

  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<SalaryFormulaEditFormValues>({
    resolver: zodResolver(buildSalaryFormulaEditSchema(t, formulaTypeValue)),
    defaultValues,
  });

  const { fields, append, remove } = useFieldArray({ control, name: 'tiers' });

  // Reset once the formula's own data arrives, so the form always starts from its actual saved
  // values rather than the empty defaults used before the fetch resolves.
  useEffect(() => {
    if (!formula) return;
    reset({
      fixedMonthlyAmount: formula.fixedMonthlyAmount != null ? Number(formula.fixedMonthlyAmount) : undefined,
      tiers:
        formula.tiers && formula.tiers.length > 0
          ? formula.tiers.map((tier) => ({
              minOrders: Number(tier.minOrders),
              maxOrders: tier.maxOrders != null ? Number(tier.maxOrders) : undefined,
              rateType: tier.rateType === 'FixedTotal' ? 1 : 0,
              rate: Number(tier.rate),
            }))
          : [emptyTier],
    });
  }, [formula, reset]);

  const handleClose = () => {
    reset(defaultValues);
    onClose();
  };

  const onSubmit = async (values: SalaryFormulaEditFormValues) => {
    if (!formulaId) return;
    try {
      await updateFormula({
        id: formulaId,
        updateSalaryFormulaCommand: {
          id: formulaId,
          fixedMonthlyAmount: formulaTypeValue === FIXED_MONTHLY ? values.fixedMonthlyAmount : null,
          tiers:
            formulaTypeValue === TIERED_PER_ORDER
              ? values.tiers.map((tier) => ({
                  minOrders: tier.minOrders,
                  maxOrders: tier.maxOrders ?? null,
                  rateType: tier.rateType,
                  rate: tier.rate,
                }))
              : [],
        },
      }).unwrap();
      toast.success(t('updateSuccess'));
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{t('detailDialog.title')}</DialogTitle>
      <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
        <DialogContent>
          {isLoading ? (
            <Stack sx={{ alignItems: 'center', py: 4 }}>
              <CircularProgress size={28} />
            </Stack>
          ) : error ? (
            <Typography color="error">{getApiErrorMessages(error, t('genericError')).join('\n')}</Typography>
          ) : formula ? (
            <Stack spacing={2} sx={{ mt: 1 }}>
              <Stack direction="row" spacing={4} sx={{ flexWrap: 'wrap', rowGap: 2 }}>
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    {t('detailDialog.platform')}
                  </Typography>
                  <Typography variant="body1">{formula.platformName ?? t('genericDefault')}</Typography>
                </Box>
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    {t('detailDialog.formulaType')}
                  </Typography>
                  <Typography variant="body1">{t(`formulaTypes.${formula.formulaType}`)}</Typography>
                </Box>
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    {t('detailDialog.effectiveFrom')}
                  </Typography>
                  <Typography variant="body1">
                    {formula.effectiveFrom ? formatDate(formula.effectiveFrom, i18n.language) : '—'}
                  </Typography>
                </Box>
                <Box>
                  <Typography variant="caption" color="text.secondary" component="div">
                    {t('detailDialog.status')}
                  </Typography>
                  {isActive ? (
                    <Chip size="small" color="success" label={t('active')} />
                  ) : (
                    <Chip size="small" label={formula.effectiveTo ? formatDate(formula.effectiveTo, i18n.language) : ''} />
                  )}
                </Box>
              </Stack>

              {!isActive && (
                <Typography variant="body2" color="text.secondary">
                  {t('detailDialog.deactivatedNotice')}
                </Typography>
              )}
              {isActive && !canEdit && (
                <Typography variant="body2" color="text.secondary">
                  {t('detailDialog.viewOnlyNotice')}
                </Typography>
              )}

              <Divider />

              {formulaTypeValue === FIXED_MONTHLY ? (
                <TextField
                  {...register('fixedMonthlyAmount', { valueAsNumber: true })}
                  label={t('createDialog.fixedMonthlyAmount')}
                  type="number"
                  required={isEditable}
                  fullWidth
                  disabled={!isEditable}
                  slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
                  error={!!errors.fixedMonthlyAmount}
                  helperText={errors.fixedMonthlyAmount?.message}
                />
              ) : (
                <Stack spacing={1.5}>
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
                        disabled={!isEditable}
                        slotProps={{ htmlInput: { min: 0 } }}
                        error={!!errors.tiers?.[index]?.minOrders}
                      />
                      <TextField
                        {...register(`tiers.${index}.maxOrders`, { valueAsNumber: true })}
                        label={t('createDialog.maxOrders')}
                        type="number"
                        size="small"
                        disabled={!isEditable}
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
                            disabled={!isEditable}
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
                        disabled={!isEditable}
                        slotProps={{ htmlInput: { min: 0, step: 0.01 } }}
                        error={!!errors.tiers?.[index]?.rate}
                      />
                      {isEditable && (
                        <IconButton
                          onClick={() => remove(index)}
                          disabled={fields.length === 1}
                          aria-label={t('createDialog.removeTier')}
                          size="small"
                        >
                          <DeleteOutlineIcon fontSize="small" />
                        </IconButton>
                      )}
                    </Stack>
                  ))}
                  {isEditable && (
                    <Button
                      startIcon={<AddIcon />}
                      onClick={() => append(emptyTier)}
                      size="small"
                      sx={{ alignSelf: 'flex-start' }}
                    >
                      {t('createDialog.addTier')}
                    </Button>
                  )}
                </Stack>
              )}
            </Stack>
          ) : null}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>{t('common:actions.cancel')}</Button>
          {isEditable && (
            <Button
              type="submit"
              variant="contained"
              disabled={saving}
              startIcon={saving ? <CircularProgress size={18} color="inherit" /> : undefined}
            >
              {t('detailDialog.save')}
            </Button>
          )}
        </DialogActions>
      </Box>
    </Dialog>
  );
}
