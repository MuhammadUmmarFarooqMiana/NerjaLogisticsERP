import { useEffect, useState } from 'react';
import { Box, Toolbar } from '@mui/material';
import { Outlet } from 'react-router-dom';
import { useIdleLogout } from '../../features/auth/useIdleLogout';
import { Breadcrumbs } from './Breadcrumbs';
import { TopBar } from './TopBar';
import { Sidebar } from './Sidebar';
import { DRAWER_WIDTH, DRAWER_WIDTH_COLLAPSED } from './layout.constants';

const COLLAPSE_STORAGE_KEY = 'nerja-sidebar-collapsed';

export default function AppShell() {
  // Only mounted while ProtectedRoute considers the user authenticated, so
  // this is exactly the right scope for the inactivity timer to run in.
  useIdleLogout();

  const [mobileOpen, setMobileOpen] = useState(false);
  const [collapsed, setCollapsed] = useState(() => window.localStorage.getItem(COLLAPSE_STORAGE_KEY) === 'true');

  useEffect(() => {
    window.localStorage.setItem(COLLAPSE_STORAGE_KEY, String(collapsed));
  }, [collapsed]);

  const sidebarWidth = collapsed ? DRAWER_WIDTH_COLLAPSED : DRAWER_WIDTH;

  return (
    <Box sx={{ display: 'flex' }}>
      <TopBar onMenuClick={() => setMobileOpen((open) => !open)} sidebarWidth={sidebarWidth} />
      <Sidebar
        mobileOpen={mobileOpen}
        onClose={() => setMobileOpen(false)}
        collapsed={collapsed}
        onToggleCollapsed={() => setCollapsed((c) => !c)}
      />
      <Box component="main" sx={{ flexGrow: 1, p: 3, width: { xs: '100%', md: 'auto' } }}>
        <Toolbar />
        <Breadcrumbs />
        <Outlet />
      </Box>
    </Box>
  );
}
