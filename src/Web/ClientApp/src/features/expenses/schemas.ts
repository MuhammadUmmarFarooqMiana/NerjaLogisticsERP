import { z } from 'zod';
import type { TFunction } from 'i18next';

// Mirrors CreateExpenseCommandValidator (Application/Expenses/Commands/CreateExpense).
export function buildExpenseSchema(t: TFunction) {
  return z.object({
    category: z.number({ error: t('common:validation.required') }),
    amount: z.number({ error: t('common:validation.required') }).gt(0, t('validation.amountPositive')),
    expenseDate: z.string().min(1, t('common:validation.required')),
    description: z.string().optional(),
    platformId: z.string().optional(),
  });
}

export type ExpenseFormValues = z.infer<ReturnType<typeof buildExpenseSchema>>;
