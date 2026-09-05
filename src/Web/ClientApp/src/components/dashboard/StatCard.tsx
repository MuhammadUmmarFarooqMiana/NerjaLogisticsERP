import ArrowForwardIcon from '@mui/icons-material/ArrowForward';
import { Box, Link, Paper, Typography } from '@mui/material';
import type { SvgIconProps } from '@mui/material';
import type { ComponentType } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

interface StatCardProps {
  icon: ComponentType<SvgIconProps>;
  label: string;
  value: string;
  color: string;
  /** Omit while the destination module hasn't shipped yet — shows a disabled "coming soon" footer instead of a dead link. */
  href?: string;
}

export function StatCard({ icon: Icon, label, value, color, href }: StatCardProps) {
  const { t } = useTranslation();

  return (
    <Paper
      variant="outlined"
      sx={{ position: 'relative', overflow: 'hidden', borderRadius: '20px', height: '100%', display: 'flex', flexDirection: 'column' }}
    >
      <Box sx={{ p: 3, flexGrow: 1 }}>
        <Box
          sx={{
            display: 'inline-flex',
            alignItems: 'center',
            justifyContent: 'center',
            width: 44,
            height: 44,
            borderRadius: 1.5,
            bgcolor: color,
            color: '#FFFFFF',
            mb: 2,
          }}
        >
          <Icon fontSize="small" />
        </Box>
        <Typography variant="body2" color="text.secondary" noWrap>
          {label}
        </Typography>
        <Typography variant="h5" fontWeight={700} sx={{ mt: 0.5 }}>
          {value}
        </Typography>
      </Box>

      <Box sx={{ bgcolor: 'action.hover', px: 3, py: 1.5, borderTop: '1px solid', borderColor: 'divider' }}>
        {href ? (
          <Link
            component={RouterLink}
            to={href}
            underline="hover"
            variant="body2"
            sx={{ fontWeight: 600, display: 'inline-flex', alignItems: 'center', gap: 0.5 }}
          >
            {t('dashboard.viewAll')}
            <ArrowForwardIcon sx={{ fontSize: 16 }} />
          </Link>
        ) : (
          <Typography variant="body2" color="text.disabled">
            {t('dashboard.comingSoon')}
          </Typography>
        )}
      </Box>
    </Paper>
  );
}
