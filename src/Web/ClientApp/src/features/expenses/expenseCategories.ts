// Mirrors Domain/Enums/ExpenseCategory.cs exactly — the API binds/returns this
// enum by its numeric ordinal (no JsonStringEnumConverter is configured), so
// order here must match the C# declaration order.
export const EXPENSE_CATEGORIES = [
  { value: 0, key: 'Fuel' },
  { value: 1, key: 'BikeRepairs' },
  { value: 2, key: 'OfficeExpenses' },
  { value: 3, key: 'SimCards' },
  { value: 4, key: 'Uniforms' },
  { value: 5, key: 'Helmets' },
  { value: 6, key: 'GeneralMaintenance' },
  { value: 7, key: 'Other' },
] as const;

export type ExpenseCategoryKey = (typeof EXPENSE_CATEGORIES)[number]['key'];
