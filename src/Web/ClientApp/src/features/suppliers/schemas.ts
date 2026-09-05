import { z } from 'zod';
import type { TFunction } from 'i18next';

// Mirrors CreateSupplierCommandValidator (Application/Suppliers/Commands/CreateSupplier).
// UpdateSupplierCommandValidator uses tighter MaxLength(150)/MaxLength(500) for
// name/address, but we validate against the stricter Create limits everywhere
// so the same form works for both without surprising a user mid-edit.
export function buildSupplierSchema(t: TFunction) {
  return z.object({
    name: z
      .string()
      .min(1, t('common:validation.required'))
      .max(150, t('common:validation.maxLength', { max: 150 })),
    email: z.union([z.literal(''), z.string().email(t('validation.emailInvalid'))]).optional(),
    phone: z.string().max(30, t('common:validation.maxLength', { max: 30 })).optional(),
    address: z.string().max(500, t('common:validation.maxLength', { max: 500 })).optional(),
  });
}

export type SupplierFormValues = z.infer<ReturnType<typeof buildSupplierSchema>>;
