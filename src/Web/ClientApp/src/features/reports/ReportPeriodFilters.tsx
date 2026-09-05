import { MenuItem, TextField } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { AppDatePicker } from '../../components/shared/AppDatePicker';
import { REPORT_PERIOD_TYPES } from './reportConstants';

const DAILY = 0;
const WEEKLY = 1;
const MONTHLY = 2;
const CUSTOM = 3;

interface ReportPeriodFiltersProps {
  periodType: number;
  onPeriodTypeChange: (value: number) => void;
  date: string;
  onDateChange: (value: string) => void;
  year: number;
  onYearChange: (value: number) => void;
  month: number;
  onMonthChange: (value: number) => void;
  startDate: string;
  onStartDateChange: (value: string) => void;
  endDate: string;
  onEndDateChange: (value: string) => void;
}

/** The Daily/Weekly/Monthly/Custom period selector + its conditional date inputs —
 * identical across every report, since they all resolve through the same backend
 * ReportPeriodResolver. */
export function ReportPeriodFilters({
  periodType,
  onPeriodTypeChange,
  date,
  onDateChange,
  year,
  onYearChange,
  month,
  onMonthChange,
  startDate,
  onStartDateChange,
  endDate,
  onEndDateChange,
}: ReportPeriodFiltersProps) {
  const { t } = useTranslation('reports');

  return (
    <>
      <TextField
        select
        size="small"
        label={t('filters.periodType')}
        value={periodType}
        onChange={(event) => onPeriodTypeChange(Number(event.target.value))}
        sx={{ minWidth: 160 }}
      >
        {REPORT_PERIOD_TYPES.map((option) => (
          <MenuItem key={option.value} value={option.value}>
            {t(`periodTypes.${option.key}`)}
          </MenuItem>
        ))}
      </TextField>

      {(periodType === DAILY || periodType === WEEKLY) && (
        <AppDatePicker
          label={periodType === DAILY ? t('filters.date') : t('filters.weekOf')}
          value={date}
          onChange={onDateChange}
          slotProps={{ textField: { size: 'small' } }}
        />
      )}

      {periodType === MONTHLY && (
        <>
          <TextField
            label={t('filters.year')}
            type="number"
            size="small"
            value={year}
            onChange={(event) => onYearChange(Number(event.target.value))}
            sx={{ width: 120 }}
          />
          <TextField
            select
            label={t('filters.month')}
            size="small"
            value={month}
            onChange={(event) => onMonthChange(Number(event.target.value))}
            sx={{ width: 160 }}
          >
            {Array.from({ length: 12 }, (_, i) => i + 1).map((m) => (
              <MenuItem key={m} value={m}>
                {t(`months.${m}`)}
              </MenuItem>
            ))}
          </TextField>
        </>
      )}

      {periodType === CUSTOM && (
        <>
          <AppDatePicker
            label={t('filters.startDate')}
            value={startDate}
            onChange={onStartDateChange}
            maxDate={endDate || undefined}
            slotProps={{ textField: { size: 'small' } }}
          />
          <AppDatePicker
            label={t('filters.endDate')}
            value={endDate}
            onChange={onEndDateChange}
            minDate={startDate || undefined}
            slotProps={{ textField: { size: 'small' } }}
          />
        </>
      )}
    </>
  );
}
