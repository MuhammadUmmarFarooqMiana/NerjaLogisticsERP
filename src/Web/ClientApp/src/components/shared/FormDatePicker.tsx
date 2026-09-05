import dayjs from 'dayjs';
import { Controller, type Control, type FieldPath, type FieldValues } from 'react-hook-form';
import { DatePicker, type DatePickerProps } from '@mui/x-date-pickers/DatePicker';

// The app's forms/schemas/API all move DateOnly values as plain 'YYYY-MM-DD'
// strings (see lib/formatDate.ts) — this wraps MUI's DatePicker (which speaks
// Dayjs objects) so every call site keeps working with that same string
// shape via react-hook-form's Controller, instead of each field re-deriving
// its own Dayjs<->string conversion.
interface FormDatePickerProps<TFieldValues extends FieldValues> {
  name: FieldPath<TFieldValues>;
  control: Control<TFieldValues>;
  label: string;
  required?: boolean;
  disabled?: boolean;
  minDate?: string;
  maxDate?: string;
  slotProps?: DatePickerProps['slotProps'];
}

export function FormDatePicker<TFieldValues extends FieldValues>({
  name,
  control,
  label,
  required,
  disabled,
  minDate,
  maxDate,
  slotProps,
}: FormDatePickerProps<TFieldValues>) {
  return (
    <Controller
      name={name}
      control={control}
      render={({ field, fieldState }) => (
        <DatePicker
          label={label}
          value={field.value ? dayjs(field.value as string) : null}
          onChange={(newValue) => field.onChange(newValue?.isValid() ? newValue.format('YYYY-MM-DD') : '')}
          disabled={disabled}
          minDate={minDate ? dayjs(minDate) : undefined}
          maxDate={maxDate ? dayjs(maxDate) : undefined}
          slotProps={{
            ...slotProps,
            textField: {
              fullWidth: true,
              required,
              error: !!fieldState.error,
              helperText: fieldState.error?.message,
              onBlur: field.onBlur,
              ...slotProps?.textField,
            },
          }}
        />
      )}
    />
  );
}
