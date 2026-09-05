import dayjs from 'dayjs';
import { DatePicker, type DatePickerProps } from '@mui/x-date-pickers/DatePicker';

// Plain controlled counterpart to FormDatePicker, for the app's non-react-hook-form
// call sites (page-level filters backed by useState) — same 'YYYY-MM-DD' string
// contract as the rest of the app (see lib/formatDate.ts), just without a Controller.
interface AppDatePickerProps
  extends Omit<DatePickerProps, 'value' | 'onChange' | 'minDate' | 'maxDate'> {
  value: string;
  onChange: (value: string) => void;
  minDate?: string;
  maxDate?: string;
}

export function AppDatePicker({ value, onChange, minDate, maxDate, ...props }: AppDatePickerProps) {
  return (
    <DatePicker
      value={value ? dayjs(value) : null}
      onChange={(newValue) => onChange(newValue?.isValid() ? newValue.format('YYYY-MM-DD') : '')}
      minDate={minDate ? dayjs(minDate) : undefined}
      maxDate={maxDate ? dayjs(maxDate) : undefined}
      {...props}
    />
  );
}
