// Shared label formatter for employee-picker dropdowns (Add Fine, Add Advance,
// Allocate Vehicle, Generate Monthly Summary, and the matching list filters).
// Iqama number is the one identifier riders/employees always recognize their
// own record by, and it disambiguates employees who share a name.
export function employeeOptionLabel(employee: { fullName?: string | null; iqamaNumber?: string | null }): string {
  const name = employee.fullName ?? '';
  return employee.iqamaNumber ? `${name} — ${employee.iqamaNumber}` : name;
}
