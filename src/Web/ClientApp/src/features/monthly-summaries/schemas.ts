import { z } from 'zod';
import type { TFunction } from 'i18next';

// Mirrors GenerateMonthlySummaryCommandValidator (Application/MonthlySummaries/Commands/GenerateMonthlySummary).
export function buildGenerateSummarySchema(t: TFunction) {
  return z.object({
    employeeId: z.string().min(1, t('common:validation.required')),
    year: z.number({ error: t('common:validation.required') }).gt(2020, t('validation.yearInvalid')),
    month: z
      .number({ error: t('common:validation.required') })
      .min(1, t('validation.monthInvalid'))
      .max(12, t('validation.monthInvalid')),
  });
}

export type GenerateSummaryFormValues = z.infer<ReturnType<typeof buildGenerateSummarySchema>>;
