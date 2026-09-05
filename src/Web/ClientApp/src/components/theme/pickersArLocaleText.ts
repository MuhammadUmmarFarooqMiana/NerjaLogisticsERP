import type { PickersLocaleText } from '@mui/x-date-pickers/locales';

// @mui/x-date-pickers doesn't ship an Arabic locale bundle (only enUS is guaranteed
// present; community translations for 'ar' aren't published for this package), so the
// picker's own internal strings (open-picker button, month/day view names, toolbar
// titles, ...) fall back to English even once dayjs's adapterLocale is set to 'ar' —
// adapterLocale only drives calendar/month-name formatting, not these UI strings.
// This fills that gap for LocalizationProvider's `localeText` prop. Field placeholder
// keys (fieldYearPlaceholder, etc.) are left as MUI's defaults on purpose: they're
// locale-neutral letter markers (YYYY/MM/DD) shown only in an empty field, not words.
export const pickersArLocaleText: Partial<PickersLocaleText> = {
  // Calendar navigation
  previousMonth: 'الشهر السابق',
  nextMonth: 'الشهر التالي',
  // View navigation
  openPreviousView: 'فتح العرض السابق',
  openNextView: 'فتح العرض التالي',
  calendarViewSwitchingButtonAriaLabel: (view) =>
    view === 'year' ? 'عرض السنة مفتوح، التبديل إلى عرض التقويم' : 'عرض التقويم مفتوح، التبديل إلى عرض السنة',
  // DateRange labels
  start: 'البداية',
  end: 'النهاية',
  startDate: 'تاريخ البدء',
  startTime: 'وقت البدء',
  endDate: 'تاريخ الانتهاء',
  endTime: 'وقت الانتهاء',
  // Action bar
  cancelButtonLabel: 'إلغاء',
  clearButtonLabel: 'مسح',
  okButtonLabel: 'موافق',
  todayButtonLabel: 'اليوم',
  nextStepButtonLabel: 'التالي',
  // Toolbar titles
  datePickerToolbarTitle: 'اختر التاريخ',
  dateTimePickerToolbarTitle: 'اختر التاريخ والوقت',
  timePickerToolbarTitle: 'اختر الوقت',
  dateRangePickerToolbarTitle: 'اختر نطاق التاريخ',
  timeRangePickerToolbarTitle: 'اختر نطاق الوقت',
  // Clock labels
  clockLabelText: (view, formattedTime) =>
    `اختر ${view}. ${!formattedTime ? 'لم يتم اختيار وقت' : `الوقت المحدد هو ${formattedTime}`}`,
  hoursClockNumberText: (hours) => `${hours} ساعات`,
  minutesClockNumberText: (minutes) => `${minutes} دقائق`,
  secondsClockNumberText: (seconds) => `${seconds} ثواني`,
  // Digital clock labels
  selectViewText: (view) => `اختر ${view}`,
  // Calendar labels
  calendarWeekNumberHeaderLabel: 'رقم الأسبوع',
  calendarWeekNumberAriaLabelText: (weekNumber) => `الأسبوع ${weekNumber}`,
  // Open Picker labels
  openDatePickerDialogue: (formattedDate) =>
    formattedDate ? `اختر التاريخ، التاريخ المحدد هو ${formattedDate}` : 'اختر التاريخ',
  openTimePickerDialogue: (formattedTime) =>
    formattedTime ? `اختر الوقت، الوقت المحدد هو ${formattedTime}` : 'اختر الوقت',
  openRangePickerDialogue: (formattedRange) =>
    formattedRange ? `اختر النطاق، النطاق المحدد هو ${formattedRange}` : 'اختر النطاق',
  fieldClearLabel: 'مسح',
  // Table labels
  timeTableLabel: 'اختيار الوقت',
  dateTableLabel: 'اختيار التاريخ',
  // View names
  year: 'سنة',
  month: 'شهر',
  day: 'يوم',
  weekDay: 'يوم الأسبوع',
  hours: 'ساعات',
  minutes: 'دقائق',
  seconds: 'ثواني',
  meridiem: 'الفترة',
  // Common
  empty: 'فارغ',
};
