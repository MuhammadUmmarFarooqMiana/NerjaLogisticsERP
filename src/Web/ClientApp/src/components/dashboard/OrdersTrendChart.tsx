import { Box, Paper, Typography, useTheme } from '@mui/material';
import { useTranslation } from 'react-i18next';
import {
  CartesianGrid,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts';
import { BRAND_ORANGE } from '../theme/theme';

export interface OrdersTrendPoint {
  date: string;
  label: string;
  completedOrders: number;
}

interface OrdersTrendChartProps {
  data: OrdersTrendPoint[];
  isLoading: boolean;
  title: string;
}

export function OrdersTrendChart({ data, isLoading, title }: OrdersTrendChartProps) {
  const { t } = useTranslation();
  const theme = useTheme();

  return (
    <Paper variant="outlined" sx={{ p: 3, borderRadius: '20px', height: '100%' }}>
      <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 2 }}>
        {title}
      </Typography>
      {isLoading ? (
        <Box sx={{ height: 280 }} />
      ) : data.every((point) => point.completedOrders === 0) ? (
        <Box sx={{ height: 280, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
          <Typography variant="body2" color="text.secondary">
            {t('dashboard.charts.noData')}
          </Typography>
        </Box>
      ) : (
        <Box sx={{ height: 280 }}>
          <ResponsiveContainer width="100%" height="100%">
            <LineChart data={data} margin={{ top: 4, right: 8, left: -16, bottom: 0 }}>
              <CartesianGrid strokeDasharray="3 3" stroke={theme.palette.divider} />
              <XAxis dataKey="label" tick={{ fontSize: 12 }} stroke={theme.palette.text.secondary} />
              <YAxis allowDecimals={false} tick={{ fontSize: 12 }} stroke={theme.palette.text.secondary} />
              <Tooltip
                contentStyle={{
                  backgroundColor: theme.palette.background.paper,
                  border: `1px solid ${theme.palette.divider}`,
                  borderRadius: 8,
                }}
                labelStyle={{ color: theme.palette.text.primary }}
              />
              <Line
                type="monotone"
                dataKey="completedOrders"
                name={t('dashboard.charts.completedOrders')}
                stroke={BRAND_ORANGE}
                strokeWidth={2}
                dot={{ r: 3 }}
                activeDot={{ r: 5 }}
              />
            </LineChart>
          </ResponsiveContainer>
        </Box>
      )}
    </Paper>
  );
}
