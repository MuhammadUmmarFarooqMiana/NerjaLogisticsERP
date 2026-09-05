import { useState, type MouseEvent } from 'react';
import DarkModeOutlinedIcon from '@mui/icons-material/DarkModeOutlined';
import LightModeOutlinedIcon from '@mui/icons-material/LightModeOutlined';
import SettingsBrightnessOutlinedIcon from '@mui/icons-material/SettingsBrightnessOutlined';
import { IconButton, ListItemIcon, ListItemText, Menu, MenuItem, Tooltip } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { useThemeMode, type ThemeMode } from '../theme/ThemeModeContext';

const MODE_ICONS: Record<ThemeMode, typeof LightModeOutlinedIcon> = {
  light: LightModeOutlinedIcon,
  dark: DarkModeOutlinedIcon,
  system: SettingsBrightnessOutlinedIcon,
};

const MODES: ThemeMode[] = ['light', 'dark', 'system'];

export function ThemeModeToggle() {
  const { t } = useTranslation();
  const { mode, setMode } = useThemeMode();
  const [anchorEl, setAnchorEl] = useState<HTMLElement | null>(null);
  const open = Boolean(anchorEl);

  const CurrentIcon = MODE_ICONS[mode];

  const handleOpen = (event: MouseEvent<HTMLElement>) => setAnchorEl(event.currentTarget);
  const handleClose = () => setAnchorEl(null);
  const handleSelect = (next: ThemeMode) => {
    setMode(next);
    handleClose();
  };

  return (
    <>
      <Tooltip title={t('theme.toggle')}>
        <IconButton color="inherit" onClick={handleOpen} aria-label={t('theme.toggle')}>
          <CurrentIcon />
        </IconButton>
      </Tooltip>
      <Menu anchorEl={anchorEl} open={open} onClose={handleClose}>
        {MODES.map((option) => {
          const OptionIcon = MODE_ICONS[option];
          return (
            <MenuItem key={option} selected={option === mode} onClick={() => handleSelect(option)}>
              <ListItemIcon>
                <OptionIcon fontSize="small" />
              </ListItemIcon>
              <ListItemText>{t(`theme.${option}`)}</ListItemText>
            </MenuItem>
          );
        })}
      </Menu>
    </>
  );
}
