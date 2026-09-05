import { Chip, type ChipProps } from '@mui/material';
import { useTranslation } from 'react-i18next';

export type StatusDomain = 'accountStatus' | 'leaveStatus' | 'monthlySummaryStatus' | 'dailyOrderStatus';

type StatusColor = ChipProps['color'];

// Mirrors Domain/Enums (AccountStatus, LeaveStatus, MonthlySummaryStatus,
// DailyOrderStatus) — enum member names are sent over the wire as strings,
// so the keys here must match those C# member names exactly.
const STATUS_COLOR_MAP: Record<StatusDomain, Record<string, StatusColor>> = {
  accountStatus: {
    Incomplete: 'default',
    PendingReview: 'warning',
    Active: 'success',
    Suspended: 'warning',
    Rejected: 'error',
    Terminated: 'error',
  },
  leaveStatus: {
    Pending: 'warning',
    Approved: 'success',
    Rejected: 'error',
  },
  monthlySummaryStatus: {
    Draft: 'default',
    Verified: 'info',
    Paid: 'success',
  },
  dailyOrderStatus: {
    Open: 'info',
    Closed: 'warning',
    Approved: 'success',
    Rejected: 'error',
    OnLeave: 'warning',
  },
};

interface StatusBadgeProps {
  domain: StatusDomain;
  status: string;
}

export function StatusBadge({ domain, status }: StatusBadgeProps) {
  const { t } = useTranslation();
  const color = STATUS_COLOR_MAP[domain][status] ?? 'default';
  const label = t(`status.${domain}.${status}`, { defaultValue: status });

  return <Chip size="small" color={color} label={label} />;
}
