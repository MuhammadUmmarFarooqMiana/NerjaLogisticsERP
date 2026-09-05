import { z } from 'zod';
import type { TFunction } from 'i18next';

// Mirrors CreateAdvanceCommandValidator (Application/Advances/Commands/CreateAdvance).
export function buildAdvanceSchema(t: TFunction) {
  return z.object({
    employeeId: z.string().min(1, t('common:validation.required')),
    amount: z.number({ error: t('common:validation.required') }).gt(0, t('validation.amountPositive')),
    advanceDate: z.string().min(1, t('common:validation.required')),
    remarks: z.string().optional(),
  });
}

export type AdvanceFormValues = z.infer<ReturnType<typeof buildAdvanceSchema>>;
