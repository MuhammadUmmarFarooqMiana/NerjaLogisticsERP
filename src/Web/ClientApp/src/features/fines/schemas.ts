import { z } from 'zod';
import type { TFunction } from 'i18next';

// Mirrors CreateFineCommandValidator (Application/Fines/Commands/CreateFine).
export function buildFineSchema(t: TFunction) {
  return z.object({
    employeeId: z.string().min(1, t('common:validation.required')),
    amount: z.number({ error: t('common:validation.required') }).gt(0, t('validation.amountPositive')),
    reason: z
      .string()
      .min(1, t('common:validation.required'))
      .max(255, t('common:validation.maxLength', { max: 255 })),
    fineDate: z.string().min(1, t('common:validation.required')),
  });
}

export type FineFormValues = z.infer<ReturnType<typeof buildFineSchema>>;
