import { ToggleButton, ToggleButtonGroup } from '@mui/material';
import { useTranslation } from 'react-i18next';

export function LanguageSwitcher() {
  const { t, i18n } = useTranslation();

  return (
    <ToggleButtonGroup
      size="small"
      exclusive
      value={i18n.language}
      onChange={(_event, next: string | null) => {
        if (next) void i18n.changeLanguage(next);
      }}
      aria-label={t('language.english') + ' / ' + t('language.arabic')}
    >
      <ToggleButton value="en">{t('language.english')}</ToggleButton>
      <ToggleButton value="ar">{t('language.arabic')}</ToggleButton>
    </ToggleButtonGroup>
  );
}
