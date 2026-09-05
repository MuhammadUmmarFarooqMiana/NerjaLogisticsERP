import { z } from 'zod';
import type { TFunction } from 'i18next';

// Mirrors UpsertDailyOrderCommandValidator (Application/DailyOrders/Commands/UpsertDailyOrder).
export function buildDailyOrderSchema(t: TFunction) {
  return z.object({
    completedOrders: z
      .number({ error: t('common:validation.required') })
      .int()
      .min(0, t('common:validation.required')),
  });
}

export type DailyOrderFormValues = z.infer<ReturnType<typeof buildDailyOrderSchema>>;

// Mirrors RejectDailyOrderCommandValidator (Application/DailyOrders/Commands/RejectDailyOrder).
export function buildRejectDailyOrderReasonSchema(t: TFunction) {
  return z.object({
    reason: z
      .string()
      .min(1, t('common:validation.required'))
      .max(5000, t('common:validation.maxLength', { max: 5000 })),
  });
}

export type RejectDailyOrderFormValues = z.infer<ReturnType<typeof buildRejectDailyOrderReasonSchema>>;

// Mirrors CorrectDailyOrderCommandValidator (Application/DailyOrders/Commands/CorrectDailyOrder).
export function buildCorrectDailyOrderSchema(t: TFunction) {
  return z.object({
    completedOrders: z
      .number({ error: t('common:validation.required') })
      .int()
      .min(0, t('common:validation.required')),
    reason: z
      .string()
      .min(1, t('common:validation.required'))
      .max(5000, t('common:validation.maxLength', { max: 5000 })),
  });
}

export type CorrectDailyOrderFormValues = z.infer<ReturnType<typeof buildCorrectDailyOrderSchema>>;
