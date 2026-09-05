import { useState } from 'react';
import NotificationsNoneOutlinedIcon from '@mui/icons-material/NotificationsNoneOutlined';
import {
  Badge,
  Box,
  Button,
  Divider,
  IconButton,
  List,
  ListItemButton,
  ListItemText,
  Menu,
  Typography,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import {
  useGetApiNotificationsQuery,
  usePostApiNotificationsByIdReadMutation,
} from '../../api/notificationsApi';
import type { NotificationDto } from '../../api/generated/apiSlice';

// Only types with a route every recipient role can reach belong here — e.g. "SalaryReady"
// targets Riders, who have no monthly-summaries page, so it's intentionally left unmapped.
const NOTIFICATION_TYPE_ROUTES: Record<string, string> = {
  LeaveRequestSubmitted: '/leave-requests',
  LeaveApproved: '/leave-requests',
  LeaveRejected: '/leave-requests',
  ProfilePendingReview: '/employees/pending',
  AccountApproved: '/profile',
  ProfileRejected: '/profile',
  // Every recipient of these — Rider (their own day) or Administrator/Supervisor
  // (reviewing someone else's) — can reach Daily Orders.
  DailyOrderAwaitingApproval: '/daily-orders',
  DailyOrderApproved: '/daily-orders',
  DailyOrderRejected: '/daily-orders',
  DailyOrderCorrected: '/daily-orders',
  DailyOrdersPendingApproval: '/daily-orders',
};

function relativeTime(iso: string, t: ReturnType<typeof useTranslation<'notifications'>>['t']): string {
  const diffMs = Date.now() - new Date(iso).getTime();
  const diffSec = Math.max(0, Math.floor(diffMs / 1000));
  if (diffSec < 60) return t('time.justNow');
  const diffMin = Math.floor(diffSec / 60);
  if (diffMin < 60) return t('time.minutesAgo', { count: diffMin });
  const diffHour = Math.floor(diffMin / 60);
  if (diffHour < 24) return t('time.hoursAgo', { count: diffHour });
  const diffDay = Math.floor(diffHour / 24);
  return t('time.daysAgo', { count: diffDay });
}

export function NotificationBell() {
  const { t } = useTranslation('notifications');
  const navigate = useNavigate();
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);
  const { data: notifications = [] } = useGetApiNotificationsQuery(
    {},
    { pollingInterval: 30000 }
  );
  const [markAsRead] = usePostApiNotificationsByIdReadMutation();

  const unreadCount = notifications.filter((n) => !n.isRead).length;
  const open = Boolean(anchorEl);

  const handleItemClick = (notification: NotificationDto) => {
    if (!notification.isRead) {
      void markAsRead({ id: notification.id });
    }
    setAnchorEl(null);
    const path = NOTIFICATION_TYPE_ROUTES[notification.type];
    if (path) {
      navigate(path);
    }
  };

  const handleMarkAllAsRead = () => {
    notifications.filter((n) => !n.isRead).forEach((n) => void markAsRead({ id: n.id }));
  };

  return (
    <>
      <IconButton
        color="inherit"
        aria-label={t('title')}
        onClick={(e) => setAnchorEl(e.currentTarget)}
      >
        <Badge badgeContent={unreadCount} color="error" max={99}>
          <NotificationsNoneOutlinedIcon />
        </Badge>
      </IconButton>
      <Menu
        anchorEl={anchorEl}
        open={open}
        onClose={() => setAnchorEl(null)}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
        transformOrigin={{ vertical: 'top', horizontal: 'right' }}
        slotProps={{ paper: { sx: { width: 360, maxHeight: 480 } } }}
      >
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', px: 2, py: 1 }}>
          <Typography variant="subtitle1" sx={{ fontWeight: 600 }}>
            {t('title')}
          </Typography>
          {unreadCount > 0 && (
            <Button size="small" onClick={handleMarkAllAsRead}>
              {t('markAllAsRead')}
            </Button>
          )}
        </Box>
        <Divider />
        {notifications.length === 0 ? (
          <Box sx={{ px: 2, py: 3 }}>
            <Typography variant="body2" color="text.secondary">
              {t('empty')}
            </Typography>
          </Box>
        ) : (
          <List sx={{ py: 0 }}>
            {notifications.map((notification) => (
              <ListItemButton
                key={notification.id}
                onClick={() => handleItemClick(notification)}
                sx={{
                  alignItems: 'flex-start',
                  bgcolor: notification.isRead ? undefined : 'action.hover',
                }}
              >
                <ListItemText
                  primary={
                    <Typography
                      variant="body2"
                      sx={{ fontWeight: notification.isRead ? 400 : 600 }}
                    >
                      {notification.title}
                    </Typography>
                  }
                  secondary={
                    <>
                      <Typography
                        component="span"
                        variant="body2"
                        color="text.secondary"
                        sx={{ display: 'block' }}
                      >
                        {notification.message}
                      </Typography>
                      <Typography component="span" variant="caption" color="text.disabled">
                        {relativeTime(notification.created, t)}
                      </Typography>
                    </>
                  }
                />
              </ListItemButton>
            ))}
          </List>
        )}
      </Menu>
    </>
  );
}
