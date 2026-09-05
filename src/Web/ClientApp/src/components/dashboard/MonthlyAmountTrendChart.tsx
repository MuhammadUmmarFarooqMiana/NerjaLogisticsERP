import { Box, Paper, Typography, useTheme } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { Bar, BarChart, CartesianGrid, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';

export interface MonthlyAmountPoint {
  month: string;
  label: string;
  amount: number;
}

interface MonthlyAmountTrendChartProps {
  data: MonthlyAmountPoint[];
  isLoading: boolean;
  title: string;
  /** Series name shown in the tooltip (e.g. "Expenses", "Salary Paid"). */
  barName: string;
  /** Bar fill color — kept per-usage rather than baked in, since this chart now
   * backs more than one domain (Expenses trend, Salary Paid trend, ...). */
  color: string;
}

/** Generic "amount by month" bar chart — originally Expenses-only, generalized so the
 * Salary Paid trend (and any future monthly-amount trend) can reuse it instead of a
 * near-duplicate component that would only differ by color and a label. */
export function MonthlyAmountTrendChart({ data, isLoading, title, barName, color }: MonthlyAmountTrendChartProps) {
  const { t } = useTranslation();
  const theme = useTheme();

  return (
    <Paper variant="outlined" sx={{ p: 3, borderRadius: '20px', height: '100%' }}>
      <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 2 }}>
        {title}
      </Typography>
      {isLoading ? (
        <Box sx={{ height: 280 }} />
      ) : data.every((point) => point.amount === 0) ? (
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
              <Bar dataKey="amount" name={barName} fill={color} radius={[4, 4, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </Box>
      )}
    </Paper>
  );
}
