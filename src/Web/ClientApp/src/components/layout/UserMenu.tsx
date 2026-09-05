import { useState } from 'react';
import LockResetIcon from '@mui/icons-material/LockReset';
import LogoutIcon from '@mui/icons-material/Logout';
import PersonOutlineIcon from '@mui/icons-material/PersonOutlineOutlined';
import { Box, Divider, IconButton, ListItemIcon, ListItemText, Menu, MenuItem, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../app/hooks';
import { usePostApiAuthLogoutMutation } from '../../api/generated/apiSlice';
import { ChangePasswordDialog } from '../../features/auth/ChangePasswordDialog';
import { loggedOut, selectCurrentUser } from '../../features/auth/authSlice';
import { useToast } from '../feedback/ToastContext';
import { UserAvatar } from './UserAvatar';

export function UserMenu() {
  const { t } = useTranslation();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const toast = useToast();
  const user = useAppSelector(selectCurrentUser);
  const [logout] = usePostApiAuthLogoutMutation();

  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);
  const [changePasswordOpen, setChangePasswordOpen] = useState(false);
  const open = Boolean(anchorEl);

  const closeMenu = () => setAnchorEl(null);

  const handleMyProfile = () => {
    closeMenu();
    navigate('/profile');
  };

  const handleChangePassword = () => {
    closeMenu();
    setChangePasswordOpen(true);
  };

  const handleLogout = async () => {
    closeMenu();
    try {
      await logout().unwrap();
    } finally {
      dispatch(loggedOut());
      toast.success(t('actions.logoutSuccess'));
      navigate('/login', { replace: true });
    }
  };

  return (
    <>
      <IconButton
        onClick={(e) => setAnchorEl(e.currentTarget)}
        aria-label={t('actions.userMenu')}
        aria-haspopup="true"
        size="small"
        sx={{ ml: 0.5 }}
      >
        <UserAvatar size={36} />
      </IconButton>
      <Menu
        anchorEl={anchorEl}
        open={open}
        onClose={closeMenu}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
        transformOrigin={{ vertical: 'top', horizontal: 'right' }}
        slotProps={{ paper: { sx: { width: 260 } } }}
      >
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, px: 2, py: 1.5 }}>
          <UserAvatar size={40} />
          <Box sx={{ minWidth: 0 }}>
            <Typography variant="subtitle2" noWrap title={user?.fullName}>
              {user?.fullName}
            </Typography>
            <Typography variant="caption" color="text.secondary" noWrap title={user?.email}>
              {user?.email}
            </Typography>
          </Box>
        </Box>
        <Divider />
        <MenuItem onClick={handleMyProfile}>
          <ListItemIcon>
            <PersonOutlineIcon fontSize="small" />
          </ListItemIcon>
          <ListItemText>{t('nav.myProfile')}</ListItemText>
        </MenuItem>
        <MenuItem onClick={handleChangePassword}>
          <ListItemIcon>
            <LockResetIcon fontSize="small" />
          </ListItemIcon>
          <ListItemText>{t('actions.changePassword')}</ListItemText>
        </MenuItem>
        <Divider />
        <MenuItem onClick={() => void handleLogout()}>
          <ListItemIcon>
            <LogoutIcon fontSize="small" />
          </ListItemIcon>
          <ListItemText>{t('actions.logout')}</ListItemText>
        </MenuItem>
      </Menu>
      <ChangePasswordDialog open={changePasswordOpen} onClose={() => setChangePasswordOpen(false)} />
    </>
  );
}
