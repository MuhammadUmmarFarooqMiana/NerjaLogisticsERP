import MenuIcon from '@mui/icons-material/Menu';
import { AppBar, Box, IconButton, Toolbar, useTheme } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { LanguageSwitcher } from './LanguageSwitcher';
import { NotificationBell } from './NotificationBell';
import { ThemeModeToggle } from './ThemeModeToggle';
import { UserMenu } from './UserMenu';

interface TopBarProps {
  onMenuClick: () => void;
  sidebarWidth: number;
}

export function TopBar({ onMenuClick, sidebarWidth }: TopBarProps) {
  const { t } = useTranslation();
  const theme = useTheme();

  return (
    <AppBar
      key={sidebarWidth}
      position="fixed"
      elevation={0}
      color="transparent"
      sx={{
        width: { md: `calc(100% - ${sidebarWidth}px)` },
        ml: { md: `${sidebarWidth}px` },
        bgcolor: 'background.paper',
        color: 'text.primary',
        borderBottom: '1px solid',
        borderColor: 'divider',
        transition: theme.transitions.create(['width', 'margin-left'], { duration: 200 }),
        zIndex: (t) => t.zIndex.drawer + 1,
      }}
    >
      <Toolbar sx={{ display: 'flex', justifyContent: 'space-between' }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <IconButton
            color="inherit"
            edge="start"
            onClick={onMenuClick}
            sx={{ display: { md: 'none' } }}
            aria-label={t('actions.openMenu')}
          >
            <MenuIcon />
          </IconButton>
        </Box>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <NotificationBell />
          <ThemeModeToggle />
          <LanguageSwitcher />
          <UserMenu />
        </Box>
      </Toolbar>
    </AppBar>
  );
}
