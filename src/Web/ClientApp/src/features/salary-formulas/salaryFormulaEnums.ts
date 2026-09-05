// Mirrors Domain/Enums/SalaryFormulaType.cs and SalaryTierRateType.cs exactly —
// the API binds/returns these by numeric ordinal (no JsonStringEnumConverter
// is configured), so order here must match the C# declaration order.
export const FORMULA_TYPES = [
  { value: 0, key: 'TieredPerOrder' },
  { value: 1, key: 'FixedMonthly' },
] as const;

export const TIER_RATE_TYPES = [
  { value: 0, key: 'PerOrder' },
  { value: 1, key: 'FixedTotal' },
] as const;

export const TIERED_PER_ORDER = 0;
export const FIXED_MONTHLY = 1;
