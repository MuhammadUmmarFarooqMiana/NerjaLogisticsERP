import { z } from 'zod';
import type { TFunction } from 'i18next';

// Mirrors CreateMechanicCommandValidator (Application/Mechanics/Commands/CreateMechanic).
// UpdateMechanicCommandValidator uses the same limits, so one schema covers both forms.
export function buildMechanicSchema(t: TFunction) {
  return z.object({
    name: z
      .string()
      .min(1, t('common:validation.required'))
      .max(150, t('common:validation.maxLength', { max: 150 })),
    phone: z.string().max(30, t('common:validation.maxLength', { max: 30 })).optional(),
    email: z.union([z.literal(''), z.string().email(t('validation.emailInvalid'))]).optional(),
    specialty: z.string().max(200, t('common:validation.maxLength', { max: 200 })).optional(),
    address: z.string().max(500, t('common:validation.maxLength', { max: 500 })).optional(),
  });
}

export type MechanicFormValues = z.infer<ReturnType<typeof buildMechanicSchema>>;
