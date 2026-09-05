import type { ReactNode } from 'react';
import { keyframes } from '@emotion/react';
import GroupsOutlinedIcon from '@mui/icons-material/GroupsOutlined';
import LocalShippingOutlinedIcon from '@mui/icons-material/LocalShippingOutlined';
import VerifiedUserOutlinedIcon from '@mui/icons-material/VerifiedUserOutlined';
import { Box, Stack, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { Logo } from '../../components/branding/Logo';
import { BRAND_ORANGE } from '../../components/theme/theme';

const FEATURE_ICONS = [LocalShippingOutlinedIcon, VerifiedUserOutlinedIcon, GroupsOutlinedIcon];

const fadeInUp = keyframes`
  from { opacity: 0; transform: translateY(12px); }
  to { opacity: 1; transform: translateY(0); }
`;

interface AuthLayoutProps {
  children: ReactNode;
}

export function AuthLayout({ children }: AuthLayoutProps) {
  const { t } = useTranslation('auth');
  const features = t('panel.features', { returnObjects: true }) as string[];

  return (
    <Box sx={{ display: 'flex', minHeight: '100dvh' }}>
      <Box
        sx={{
          display: { xs: 'none', md: 'flex' },
          flexDirection: 'column',
          justifyContent: 'space-between',
          width: '44%',
          p: 6,
          position: 'relative',
          overflow: 'hidden',
          color: '#FFFFFF',
          background: 'linear-gradient(150deg, #0E142B 0%, #1B3A8C 55%, #2E67FF 100%)',
        }}
      >
        <Box
          aria-hidden="true"
          sx={{
            position: 'absolute',
            inset: 0,
            opacity: 0.12,
            background: `
              linear-gradient(115deg, transparent 40%, ${BRAND_ORANGE} 40%, ${BRAND_ORANGE} 43%, transparent 43%),
              linear-gradient(115deg, transparent 60%, ${BRAND_ORANGE} 60%, ${BRAND_ORANGE} 62%, transparent 62%)`,
          }}
        />

        <Logo variant="white" height={26} />

        <Stack spacing={4} sx={{ position: 'relative', maxWidth: 380 }}>
          <Stack spacing={1.5}>
            <Typography variant="h4" fontWeight={700} sx={{ fontFamily: '"Open Sans", sans-serif' }}>
              {t('panel.headline')}
            </Typography>
            <Typography variant="body1" sx={{ color: 'rgba(255,255,255,0.75)' }}>
              {t('panel.subtitle')}
            </Typography>
          </Stack>

          <Stack spacing={2}>
            {features.map((feature, index) => {
              const Icon = FEATURE_ICONS[index % FEATURE_ICONS.length];
              return (
                <Stack key={feature} direction="row" spacing={1.5} sx={{ alignItems: 'center' }}>
                  <Box
                    sx={{
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      width: 36,
                      height: 36,
                      borderRadius: '50%',
                      flexShrink: 0,
                      bgcolor: 'rgba(242, 97, 31, 0.18)',
                      color: BRAND_ORANGE,
                    }}
                  >
                    <Icon fontSize="small" />
                  </Box>
                  <Typography variant="body2" sx={{ color: 'rgba(255,255,255,0.9)' }}>
                    {feature}
                  </Typography>
                </Stack>
              );
            })}
          </Stack>
        </Stack>

        <Typography variant="caption" sx={{ position: 'relative', color: 'rgba(255,255,255,0.5)' }}>
          © {new Date().getFullYear()} Nerja Logistics ERP
        </Typography>
      </Box>

      <Box
        sx={{
          flex: 1,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          bgcolor: 'background.default',
          p: 2,
        }}
      >
        <Box
          sx={{
            width: '100%',
            maxWidth: 420,
            animation: `${fadeInUp} 400ms ease-out`,
            '@media (prefers-reduced-motion: reduce)': { animation: 'none' },
          }}
        >
          <Box sx={{ display: { xs: 'flex', md: 'none' }, justifyContent: 'center', mb: 4 }}>
            <Logo variant="black" height={30} />
          </Box>
          {children}
        </Box>
      </Box>
    </Box>
  );
}
