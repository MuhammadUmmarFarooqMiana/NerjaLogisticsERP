import AccountBalanceWalletOutlinedIcon from '@mui/icons-material/AccountBalanceWalletOutlined';
import DirectionsCarFilledOutlinedIcon from '@mui/icons-material/DirectionsCarFilledOutlined';
import EventBusyOutlinedIcon from '@mui/icons-material/EventBusyOutlined';
import GavelOutlinedIcon from '@mui/icons-material/GavelOutlined';
import Inventory2OutlinedIcon from '@mui/icons-material/Inventory2Outlined';
import LocalShippingOutlinedIcon from '@mui/icons-material/LocalShippingOutlined';
import ReceiptLongOutlinedIcon from '@mui/icons-material/ReceiptLongOutlined';
import StorefrontOutlinedIcon from '@mui/icons-material/StorefrontOutlined';
import WalletOutlinedIcon from '@mui/icons-material/WalletOutlined';
import { Box, Grid, Paper, Typography } from '@mui/material';
import type { SvgIconProps } from '@mui/material';
import type { ComponentType } from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { useAppSelector } from '../../app/hooks';
import { Roles } from '../../lib/roles';
import { selectCurrentUser } from '../auth/authSlice';

interface ReportCardDef {
  key: string;
  icon: ComponentType<SvgIconProps>;
  path: string;
  /** Mirrors this report's own RoleGate in routes/index.tsx — kept in sync there. */
  roles: string[];
}

const REPORT_CARDS: ReportCardDef[] = [
  { key: 'salaries', icon: AccountBalanceWalletOutlinedIcon, path: '/reports/salaries', roles: [Roles.Administrator, Roles.Accountant] },
  { key: 'orders', icon: LocalShippingOutlinedIcon, path: '/reports/orders', roles: [Roles.Administrator, Roles.Supervisor] },
  { key: 'expenses', icon: ReceiptLongOutlinedIcon, path: '/reports/expenses', roles: [Roles.Administrator, Roles.Accountant] },
  { key: 'fines', icon: GavelOutlinedIcon, path: '/reports/fines', roles: [Roles.Administrator, Roles.Accountant] },
  { key: 'advances', icon: WalletOutlinedIcon, path: '/reports/advances', roles: [Roles.Administrator, Roles.Accountant] },
  { key: 'vehicles', icon: DirectionsCarFilledOutlinedIcon, path: '/reports/vehicles', roles: [Roles.Administrator, Roles.Accountant] },
  { key: 'leaves', icon: EventBusyOutlinedIcon, path: '/reports/leaves', roles: [Roles.Administrator, Roles.Supervisor] },
  { key: 'suppliers', icon: StorefrontOutlinedIcon, path: '/reports/suppliers', roles: [Roles.Administrator, Roles.Accountant] },
  { key: 'inventory', icon: Inventory2OutlinedIcon, path: '/reports/inventory-ledger', roles: [Roles.Administrator, Roles.Accountant] },
];

export default function ReportsHomePage() {
  const { t } = useTranslation('reports');
  const navigate = useNavigate();
  const user = useAppSelector(selectCurrentUser);

  const cards = REPORT_CARDS.filter((card) => user && card.roles.some((role) => user.roles.includes(role)));

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {t('home.title')}
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        {t('home.subtitle')}
      </Typography>

      <Grid container spacing={3}>
        {cards.map((card) => (
          <Grid key={card.key} size={{ xs: 12, sm: 6, md: 4 }}>
            <Paper
              variant="outlined"
              onClick={() => navigate(card.path)}
              sx={{
                p: 3,
                height: '100%',
                cursor: 'pointer',
                transition: 'box-shadow 150ms, transform 150ms',
                '&:hover': { boxShadow: 3, transform: 'translateY(-2px)' },
              }}
            >
              <Box
                sx={{
                  display: 'inline-flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  width: 44,
                  height: 44,
                  borderRadius: 1.5,
                  bgcolor: 'primary.main',
                  color: 'primary.contrastText',
                  mb: 2,
                }}
              >
                <card.icon fontSize="small" />
              </Box>
              <Typography variant="h6" gutterBottom>
                {t(`types.${card.key}`)}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {t('home.available')}
              </Typography>
            </Paper>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
}
