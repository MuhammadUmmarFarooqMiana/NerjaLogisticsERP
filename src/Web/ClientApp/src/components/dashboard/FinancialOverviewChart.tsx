import { Box, Paper, Typography, useTheme } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { Bar, BarChart, CartesianGrid, Cell, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';

export interface FinancialBar {
  key: 'fines' | 'advances' | 'expenses';
  label: string;
  amount: number;
  color: string;
}

interface FinancialOverviewChartProps {
  data: FinancialBar[];
  isLoading: boolean;
  title: string;
}

export function FinancialOverviewChart({ data, isLoading, title }: FinancialOverviewChartProps) {
  const { t } = useTranslation();
  const theme = useTheme();
  const total = data.reduce((sum, bar) => sum + bar.amount, 0);

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
            <BarChart data={data} margin={{ top: 4, right: 8, left: -16, bottom: 0 }}>
              <CartesianGrid strokeDasharray="3 3" stroke={theme.palette.divider} />
              <XAxis dataKey="label" tick={{ fontSize: 12 }} stroke={theme.palette.text.secondary} />
              <YAxis allowDecimals={false} tick={{ fontSize: 12 }} stroke={theme.palette.text.secondary} />
              <Tooltip
                contentStyle={{
                  backgroundColor: theme.palette.background.paper,
                  border: `1px solid ${theme.palette.divider}`,
                  borderRadius: 8,
                }}
                formatter={(value) => [`SAR ${Number(value).toLocaleString()}`, t('dashboard.charts.amount')]}
              />
              <Bar dataKey="amount" radius={[4, 4, 0, 0]}>
                {data.map((bar) => (
                  <Cell key={bar.key} fill={bar.color} />
                ))}
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        </Box>
      )}
    </Paper>
  );
}
