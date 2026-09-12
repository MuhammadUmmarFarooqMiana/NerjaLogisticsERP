import HomeOutlinedIcon from '@mui/icons-material/HomeOutlined';
import NavigateNextIcon from '@mui/icons-material/NavigateNext';
import { Box, Breadcrumbs as MuiBreadcrumbs, IconButton, Link as MuiLink, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { Link as RouterLink, useLocation } from 'react-router-dom';

// Maps a full pathname to the i18n key for its label — reuses the same nav.* keys
// Sidebar already renders, plus reports:types.* for the report sub-pages, so labels
// never drift out of sync with the nav they mirror.
const ROUTE_LABEL_KEYS: Record<string, string> = {
  '/dashboard': 'common:nav.dashboard',
  '/profile': 'common:nav.myProfile',
  '/leave-requests': 'common:nav.leaveRequests',
  '/daily-orders': 'common:nav.dailyOrders',
  '/employees': 'common:nav.employees',
  '/employees/pending': 'common:nav.pendingApprovals',
  '/fines': 'common:nav.fines',
  '/advances': 'common:nav.advances',
  '/monthly-summaries': 'common:nav.monthlySummaries',
  '/expenses': 'common:nav.expenses',
  '/salary-formulas': 'common:nav.salaryFormulas',
  '/company-documents': 'common:nav.companyDocuments',
  '/vehicles': 'common:nav.vehicles',
  '/inventory': 'common:nav.inventory',
  '/suppliers': 'common:nav.suppliers',
  '/mechanics': 'common:nav.mechanics',
  '/reports': 'common:nav.reports',
  '/reports/orders': 'reports:types.orders',
  '/reports/fines': 'reports:types.fines',
  '/reports/advances': 'reports:types.advances',
  '/reports/expenses': 'reports:types.expenses',
  '/reports/leaves': 'reports:types.leaves',
  '/reports/vehicles': 'reports:types.vehicles',
  '/reports/suppliers': 'reports:types.suppliers',
  '/reports/inventory-ledger': 'reports:types.inventory',
  '/reports/salaries': 'reports:types.salaries',
  '/audit-invoice': 'common:nav.auditInvoice',
};

function humanize(segment: string): string {
  return segment.replace(/-/g, ' ').replace(/\b\w/g, (char) => char.toUpperCase());
}

export function Breadcrumbs() {
  const { t } = useTranslation(['common', 'reports']);
  const location = useLocation();

  const segments = location.pathname.split('/').filter(Boolean);
  const crumbs: { path: string; label: string }[] = [];
  let accumulatedPath = '';

  for (const segment of segments) {
    accumulatedPath += `/${segment}`;
    const key = ROUTE_LABEL_KEYS[accumulatedPath];
    let label: string;
    if (key) {
      label = t(key);
    } else if (accumulatedPath.startsWith('/employees/')) {
      label = t('common:breadcrumbs.employeeDetails');
    } else if (accumulatedPath.startsWith('/vehicles/')) {
      label = t('common:breadcrumbs.vehicleDetails');
    } else {
      label = humanize(segment);
    }
    crumbs.push({ path: accumulatedPath, label });
  }

  // The Home icon already goes to the dashboard, so a trailing "Dashboard" crumb
  // there would just repeat it.
  if (location.pathname === '/dashboard') {
    crumbs.length = 0;
  }

  return (
    <Box sx={{ mb: 2 }}>
      <MuiBreadcrumbs
        separator={<NavigateNextIcon fontSize="small" sx={{ color: 'action.disabled' }} />}
        aria-label={t('common:breadcrumbs.home')}
      >
        <IconButton
          size="small"
          component={RouterLink}
          to="/dashboard"
          aria-label={t('common:breadcrumbs.home')}
          sx={{ p: 0.25 }}
        >
          <HomeOutlinedIcon fontSize="small" sx={{ color: 'text.secondary' }} />
        </IconButton>
        {crumbs.map((crumb, index) =>
          index === crumbs.length - 1 ? (
            <Typography key={crumb.path} variant="body2" sx={{ color: 'primary.main', fontWeight: 600 }}>
              {crumb.label}
            </Typography>
          ) : (
            <MuiLink
              key={crumb.path}
              component={RouterLink}
              to={crumb.path}
              underline="hover"
              variant="body2"
              color="text.secondary"
            >
              {crumb.label}
            </MuiLink>
          )
        )}
      </MuiBreadcrumbs>
    </Box>
  );
}
