import { z } from 'zod';
import type { TFunction } from 'i18next';
import { FIXED_MONTHLY, TIERED_PER_ORDER } from './salaryFormulaEnums';

// Mirrors CreateSalaryFormulaCommandValidator + SalaryFormula.AddTier's domain
// invariants (Application/Salaries/Commands/CreateSalaryFormula).
export function buildSalaryFormulaSchema(t: TFunction) {
  return z
    .object({
      platformId: z.string().optional(),
      formulaType: z.number(),
      fixedMonthlyAmount: z.number().optional(),
      effectiveFrom: z.string().min(1, t('common:validation.required')),
      tiers: z.array(
        z.object({
          minOrders: z.number().min(0, t('validation.minOrdersNonNegative')),
          maxOrders: z.number().optional(),
          rateType: z.number(),
          rate: z.number().min(0, t('validation.ratePositive')),
        })
      ),
    })
    .superRefine((values, ctx) => {
      if (values.formulaType === FIXED_MONTHLY) {
        if (!values.fixedMonthlyAmount || values.fixedMonthlyAmount <= 0) {
          ctx.addIssue({
            code: 'custom',
            path: ['fixedMonthlyAmount'],
            message: t('validation.fixedAmountPositive'),
          });
        }
      }
      if (values.formulaType === TIERED_PER_ORDER) {
        if (values.tiers.length === 0) {
          ctx.addIssue({ code: 'custom', path: ['tiers'], message: t('validation.tiersRequired') });
        }
        values.tiers.forEach((tier, index) => {
          if (tier.maxOrders != null && tier.maxOrders < tier.minOrders) {
            ctx.addIssue({
              code: 'custom',
              path: ['tiers', index, 'maxOrders'],
              message: t('validation.maxOrdersInvalid'),
            });
          }
        });
      }
    });
}

export type SalaryFormulaFormValues = z.infer<ReturnType<typeof buildSalaryFormulaSchema>>;

// Edit mode only lets the terms (amount or tier rates) of an existing formula change —
// Platform/FormulaType/EffectiveFrom are read-only there, so this only covers the fields
// UpdateSalaryFormulaCommand actually accepts (Application/Salaries/Commands/UpdateSalaryFormula).
export function buildSalaryFormulaEditSchema(t: TFunction, formulaType: number) {
  return z
    .object({
      fixedMonthlyAmount: z.number().optional(),
      tiers: z.array(
        z.object({
          minOrders: z.number().min(0, t('validation.minOrdersNonNegative')),
          maxOrders: z.number().optional(),
          rateType: z.number(),
          rate: z.number().min(0, t('validation.ratePositive')),
        })
      ),
    })
    .superRefine((values, ctx) => {
      if (formulaType === FIXED_MONTHLY) {
        if (!values.fixedMonthlyAmount || values.fixedMonthlyAmount <= 0) {
          ctx.addIssue({
            code: 'custom',
            path: ['fixedMonthlyAmount'],
            message: t('validation.fixedAmountPositive'),
          });
        }
      }
      if (formulaType === TIERED_PER_ORDER) {
        if (values.tiers.length === 0) {
          ctx.addIssue({ code: 'custom', path: ['tiers'], message: t('validation.tiersRequired') });
        }
        values.tiers.forEach((tier, index) => {
          if (tier.maxOrders != null && tier.maxOrders < tier.minOrders) {
            ctx.addIssue({
              code: 'custom',
              path: ['tiers', index, 'maxOrders'],
              message: t('validation.maxOrdersInvalid'),
            });
          }
        });
      }
    });
}

export type SalaryFormulaEditFormValues = z.infer<ReturnType<typeof buildSalaryFormulaEditSchema>>;
