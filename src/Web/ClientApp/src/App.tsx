import { useEffect, useState } from 'react';
import { Provider } from 'react-redux';
import { CacheProvider } from '@emotion/react';
import { CssBaseline, ThemeProvider } from '@mui/material';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import 'dayjs/locale/ar';
import { BrowserRouter } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { store } from './app/store';
import { useAppDispatch } from './app/hooks';
import { bootstrapAuth } from './features/auth/authSlice';
import { getEmotionCache } from './components/theme/rtlCache';
import { pickersArLocaleText } from './components/theme/pickersArLocaleText';
import { createAppTheme, type AppDirection } from './components/theme/theme';
import { ThemeModeProvider, useThemeMode } from './components/theme/ThemeModeContext';
import { ErrorBoundary } from './components/feedback/ErrorBoundary';
import { ToastProvider } from './components/feedback/ToastContext';
import AppRoutes from './routes';
import './i18n/config';

function Root() {
  const { i18n } = useTranslation();
  const dispatch = useAppDispatch();
  const { resolvedMode } = useThemeMode();
  const [direction, setDirection] = useState<AppDirection>(i18n.dir() as AppDirection);

  useEffect(() => {
    void dispatch(bootstrapAuth());
  }, [dispatch]);

  useEffect(() => {
    const applyDirection = () => {
      const dir = i18n.dir() as AppDirection;
      setDirection(dir);
      document.documentElement.dir = dir;
      document.documentElement.lang = i18n.language;
    };

    applyDirection();
    i18n.on('languageChanged', applyDirection);
    return () => {
      i18n.off('languageChanged', applyDirection);
    };
  }, [i18n]);

  return (
    <CacheProvider value={getEmotionCache(direction)}>
      <ThemeProvider theme={createAppTheme(direction, resolvedMode)}>
        <CssBaseline />
        <LocalizationProvider
          dateAdapter={AdapterDayjs}
          adapterLocale={i18n.language}
          localeText={i18n.language === 'ar' ? pickersArLocaleText : undefined}
        >
          <ToastProvider>
            <ErrorBoundary>
              <BrowserRouter>
                <AppRoutes />
              </BrowserRouter>
            </ErrorBoundary>
          </ToastProvider>
        </LocalizationProvider>
      </ThemeProvider>
    </CacheProvider>
  );
}

export default function App() {
  return (
    <Provider store={store}>
      <ThemeModeProvider>
        <Root />
      </ThemeModeProvider>
    </Provider>
  );
}
