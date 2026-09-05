import { z } from 'zod';
import type { TFunction } from 'i18next';

// Mirrors CreateInventoryItemCommandValidator (Application/Inventory/Commands/CreateInventoryItem).
export function buildInventoryItemSchema(t: TFunction) {
  return z.object({
    itemName: z
      .string()
      .min(1, t('common:validation.required'))
      .max(150, t('common:validation.maxLength', { max: 150 })),
    unit: z.string().min(1, t('common:validation.required')),
    reorderLevel: z.number({ error: t('common:validation.required') }).min(0, t('validation.reorderLevelNonNegative')),
  });
}

export type InventoryItemFormValues = z.infer<ReturnType<typeof buildInventoryItemSchema>>;

// Mirrors RecordStockInCommandValidator.
export function buildStockInSchema(t: TFunction) {
  return z.object({
    supplierId: z.string().min(1, t('common:validation.required')),
    quantity: z.number({ error: t('common:validation.required') }).gt(0, t('validation.quantityPositive')),
    stockDate: z.string().min(1, t('common:validation.required')),
  });
}

export type StockInFormValues = z.infer<ReturnType<typeof buildStockInSchema>>;

// Mirrors RecordStockOutCommandValidator.
export function buildStockOutSchema(t: TFunction) {
  return z.object({
    mechanicId: z.string().min(1, t('common:validation.required')),
    employeeId: z.string().min(1, t('common:validation.required')),
    quantity: z.number({ error: t('common:validation.required') }).gt(0, t('validation.quantityPositive')),
    stockDate: z.string().min(1, t('common:validation.required')),
  });
}

export type StockOutFormValues = z.infer<ReturnType<typeof buildStockOutSchema>>;
