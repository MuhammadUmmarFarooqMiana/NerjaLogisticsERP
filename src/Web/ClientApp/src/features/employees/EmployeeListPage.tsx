import { useState } from 'react';
import { Box, Tab, Tabs } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { useAppSelector } from '../../app/hooks';
import { Roles } from '../../lib/roles';
import { selectCurrentUser } from '../auth/authSlice';
import { UsersRolesView } from '../users/UsersRolesView';
import { EmployeeListView } from './EmployeeListView';

export default function EmployeeListPage() {
  const { t } = useTranslation('employees');
  const user = useAppSelector(selectCurrentUser);
  const isAdmin = !!user?.roles.some((role) => role === Roles.Administrator);
  const [tab, setTab] = useState(0);

  // Only Administrators manage users/roles, so non-admins keep today's plain,
  // tab-free employees list — no point showing a single-tab bar for everyone else.
  if (!isAdmin) {
    return <EmployeeListView title={t('list.title')} />;
  }

  return (
    <Box>
      <Tabs value={tab} onChange={(_event, value: number) => setTab(value)} sx={{ mb: 2, borderBottom: 1, borderColor: 'divider' }}>
        <Tab label={t('list.title')} />
        <Tab label={t('admin.usersTab')} />
      </Tabs>
      {tab === 0 ? <EmployeeListView title="" /> : <UsersRolesView />}
    </Box>
  );
}
