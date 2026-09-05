import type { ReactNode } from 'react';
import { Box, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';

interface DetailFieldProps {
  label: string;
  value?: string | null;
  /** Custom content in place of the plain text `value` (e.g. PerformanceIndicator). */
  children?: ReactNode;
}

export function DetailField({ label, value, children }: DetailFieldProps) {
  const { t } = useTranslation('employees');
  return (
    <Box>
      <Typography variant="caption" color="text.secondary">
        {label}
      </Typography>
      {children ?? <Typography variant="body1">{value || t('detail.notProvided')}</Typography>}
    </Box>
  );
}
