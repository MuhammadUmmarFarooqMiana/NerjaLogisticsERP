import { ToggleButton, ToggleButtonGroup } from '@mui/material';
import { useTranslation } from 'react-i18next';
import type { PlatformDto } from '../../api/generated/apiSlice';

export const ALL_PLATFORMS_VALUE = 'all';

interface PlatformFilterProps {
  platforms: PlatformDto[];
  selected: string;
  onChange: (value: string) => void;
}

export function PlatformFilter({ platforms, selected, onChange }: PlatformFilterProps) {
  const { t } = useTranslation();

  return (
    <ToggleButtonGroup
      value={selected}
      exclusive
      onChange={(_event, value: string | null) => {
        if (value) onChange(value);
      }}
      size="small"
      sx={{ flexWrap: 'wrap', gap: 1, mb: 3, '& .MuiToggleButtonGroup-grouped': { borderRadius: '16px !important', border: '1px solid' } }}
    >
      <ToggleButton value={ALL_PLATFORMS_VALUE}>{t('dashboard.allPlatforms')}</ToggleButton>
      {platforms.map((platform) => (
        <ToggleButton key={platform.id} value={platform.id ?? ''}>
          {platform.name}
        </ToggleButton>
      ))}
    </ToggleButtonGroup>
  );
}
