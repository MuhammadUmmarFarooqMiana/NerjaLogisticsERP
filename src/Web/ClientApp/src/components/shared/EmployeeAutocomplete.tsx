import { Autocomplete, TextField } from '@mui/material';
import type { EmployeeListItemDto } from '../../api/generated/apiSlice';
import { employeeOptionLabel } from '../../lib/employeeDisplay';

interface EmployeeAutocompleteProps {
  employees: EmployeeListItemDto[];
  /** Employee id, or '' for no filter (matches the plain-select filters this replaces). */
  value: string;
  onChange: (employeeId: string) => void;
  label: string;
  /** Shown as placeholder text and as the option when the field is cleared. */
  allLabel: string;
  sx?: object;
}

// Searchable replacement for the plain `<TextField select>` + `<MenuItem>` employee filter
// used across Fines/Advances/Leave Requests/Monthly Summaries/Daily Orders/Employees — typing
// filters the option list instead of requiring a scroll through every employee.
export function EmployeeAutocomplete({ employees, value, onChange, label, allLabel, sx }: EmployeeAutocompleteProps) {
  const selected = employees.find((e) => e.id === value) ?? null;

  return (
    <Autocomplete
      size="small"
      sx={{ minWidth: 320, ...sx }}
      options={employees}
      value={selected}
      onChange={(_event, newValue) => onChange(newValue?.id ?? '')}
      getOptionLabel={(option) => employeeOptionLabel(option)}
      isOptionEqualToValue={(option, val) => option.id === val.id}
      renderInput={(params) => <TextField {...params} label={label} placeholder={allLabel} />}
    />
  );
}
