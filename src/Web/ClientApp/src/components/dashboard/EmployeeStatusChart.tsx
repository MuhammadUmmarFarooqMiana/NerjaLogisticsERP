import { Box, Paper, Typography, useTheme } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { Cell, Legend, Pie, PieChart, ResponsiveContainer, Tooltip } from 'recharts';

export interface StatusSlice {
  status: string;
  count: number;
}

const STATUS_COLORS: Record<string, string> = {
  Active: '#2E7D32',
  PendingReview: '#ED6C02',
  Rejected: '#D32F2F',
  Incomplete: '#9E9E9E',
};

interface EmployeeStatusChartProps {
  data: StatusSlice[];
  isLoading: boolean;
  title: string;
}

export function EmployeeStatusChart({ data, isLoading, title }: EmployeeStatusChartProps) {
  const { t } = useTranslation();
  const theme = useTheme();
  const total = data.reduce((sum, slice) => sum + slice.count, 0);

  return (
    <Paper variant="outlined" sx={{ p: 3, borderRadius: '20px', height: '100%' }}>
      <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 2 }}>
        {title}
      </Typography>
      {isLoading ? (
        <Box sx={{ height: 280 }} />
      ) : total === 0 ? (
        <Box sx={{ height: 280, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
          <Typography variant="body2" color="text.secondary">
            {t('dashboard.charts.noData')}
          </Typography>
        </Box>
      ) : (
        <Box sx={{ height: 280 }}>
          <ResponsiveContainer width="100%" height="100%">
            <PieChart>
              <Pie
                data={data}
                dataKey="count"
                nameKey="status"
                cx="50%"
                cy="50%"
                innerRadius={55}
                outerRadius={90}
                paddingAngle={2}
              >
                {data.map((slice) => (
                  <Cell key={slice.status} fill={STATUS_COLORS[slice.status] ?? theme.palette.grey[500]} />
                ))}
              </Pie>
              <Tooltip
                contentStyle={{
                  backgroundColor: theme.palette.background.paper,
                  border: `1px solid ${theme.palette.divider}`,
                  borderRadius: 8,
                }}
                formatter={(value, name) => [
                  value,
                  t(`common:status.accountStatus.${String(name)}`, { defaultValue: String(name) }),
                ]}
              />
              <Legend
                formatter={(value: string) =>
                  t(`common:status.accountStatus.${value}`, { defaultValue: value })
                }
                wrapperStyle={{ fontSize: 12 }}
              />
            </PieChart>
          </ResponsiveContainer>
        </Box>
      )}
    </Paper>
  );
}
