import { z } from 'zod';
import type { TFunction } from 'i18next';

// Mirrors SubmitLeaveRequestCommandValidator (Application/LeaveRequest/Commands/SubmitLeaveRequest).
export function buildLeaveRequestSchema(t: TFunction) {
  return z
    .object({
      startDate: z.string().min(1, t('common:validation.required')),
      endDate: z.string().min(1, t('common:validation.required')),
      reason: z.string().optional(),
    })
    .refine((values) => !values.startDate || !values.endDate || values.endDate >= values.startDate, {
      message: t('validation.endDateBeforeStart'),
      path: ['endDate'],
    });
}

export type LeaveRequestFormValues = z.infer<ReturnType<typeof buildLeaveRequestSchema>>;

// Mirrors RejectLeaveRequestCommandValidator.
export function buildRejectReasonSchema(t: TFunction) {
  return z.object({
    reason: z
      .string()
      .min(1, t('common:validation.required'))
      .max(5000, t('common:validation.maxLength', { max: 5000 })),
  });
}

export type RejectReasonFormValues = z.infer<ReturnType<typeof buildRejectReasonSchema>>;
