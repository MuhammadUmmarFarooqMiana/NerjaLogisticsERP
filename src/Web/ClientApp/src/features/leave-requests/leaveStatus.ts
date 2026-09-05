// Mirrors Domain/Enums/LeaveStatus.cs exactly — the API binds/returns this
// enum by its numeric ordinal for query filters (no JsonStringEnumConverter
// is configured), so order here must match the C# declaration order. Note
// LeaveRequestDto.status itself comes back as a string ("Pending"/etc, via
// .ToString() in the handler) — this mapping is only needed for the filter.
export const LEAVE_STATUSES = [
  { value: 0, key: 'Pending' },
  { value: 1, key: 'Approved' },
  { value: 2, key: 'Rejected' },
] as const;
