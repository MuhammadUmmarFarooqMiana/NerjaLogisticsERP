import { Box, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { Roles } from '../../lib/roles';

export interface PlatformRoleCounts {
  platformId: string | null;
  platformName: string;
  riderCount: number;
  accountantCount: number;
  supervisorCount: number;
}

interface RidersByPlatformCardProps {
  data: PlatformRoleCounts[];
  title: string;
}

// Per-platform headcount broken down by role: a header row naming each role,
// then one row per platform with that role's count under it, e.g.
//              Rider   Accountant   Supervisor
//   Hunger       2          3            1
//
// Hidden entirely on mobile (`display: { xs: 'none', md: 'block' }`) — a table
// this wide has no useful compact form at phone width, so it's dropped rather
// than squeezed or made to scroll.
export function RidersByPlatformCard({ data, title }: RidersByPlatformCardProps) {
  const { t } = useTranslation();

  return (
    <Paper
      variant="outlined"
      sx={{ p: 3, borderRadius: '20px', height: '100%', display: { xs: 'none', md: 'block' } }}
    >
      <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 2 }}>
        {title}
      </Typography>
      {data.length === 0 ? (
        <Box sx={{ height: 200, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
          <Typography variant="body2" color="text.secondary">
            {t('dashboard.charts.noData')}
          </Typography>
        </Box>
      ) : (
        <TableContainer sx={{ overflowX: 'auto' }}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>{t('dashboard.charts.platform')}</TableCell>
                <TableCell align="right">{t(`users:roles.${Roles.Rider}`)}</TableCell>
                <TableCell align="right">{t(`users:roles.${Roles.Accountant}`)}</TableCell>
                <TableCell align="right">{t(`users:roles.${Roles.Supervisor}`)}</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {data.map((row) => (
                <TableRow key={row.platformName}>
                  <TableCell>{row.platformName}</TableCell>
                  <TableCell align="right">{row.riderCount}</TableCell>
                  <TableCell align="right">{row.accountantCount}</TableCell>
                  <TableCell align="right">{row.supervisorCount}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Paper>
  );
}
