import { Box, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';

// Mirrors Domain/Enums/PerformanceStatus — enum member names are sent over the wire
// as strings, so these keys must match those C# member names exactly.
//
// Literal traffic-light colors rather than theme palette tokens: this theme remaps
// palette.error.main to a brand orange (see theme.ts), which would make "Red" (poor)
// nearly indistinguishable from "Yellow" (needs attention) if driven through
// Chip color="error"/"warning" — the same reasoning behind the literal red used for
// Monthly Summaries' deduction amounts.
const PERFORMANCE_COLORS: Record<string, string> = {
  Green: '#2E7D32',
  Yellow: '#F9A825',
  Red: '#D32F2F',
};

interface PerformanceIndicatorProps {
  status?: string | null;
}

/** A colored dot + meaningful label — not the raw "Green"/"Yellow"/"Red" enum word. */
export function PerformanceIndicator({ status }: PerformanceIndicatorProps) {
  const { t } = useTranslation('employees');

  if (!status) {
    return <Typography variant="body1">{t('detail.notProvided')}</Typography>;
  }

  const color = PERFORMANCE_COLORS[status] ?? '#9e9e9e';
  const label = t(`detail.performanceLevels.${status}`, { defaultValue: status });

  return (
    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
      <Box sx={{ width: 10, height: 10, borderRadius: '50%', bgcolor: color, flexShrink: 0 }} />
      <Typography variant="body1" sx={{ color, fontWeight: 600 }}>
        {label}
      </Typography>
    </Box>
  );
}
