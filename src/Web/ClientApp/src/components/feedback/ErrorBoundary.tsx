import { Component, type ErrorInfo, type ReactNode } from 'react';
import { Box, Button, Stack, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';

function ErrorFallback() {
  const { t } = useTranslation();

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh', alignItems: 'center', justifyContent: 'center', p: 3 }}>
      <Stack spacing={2} sx={{ maxWidth: 420, textAlign: 'center' }}>
        <Typography variant="h5" sx={{ fontWeight: 600 }}>
          {t('common:errors.unexpectedTitle')}
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {t('common:errors.unexpectedMessage')}
        </Typography>
        <Button variant="contained" onClick={() => window.location.reload()} sx={{ alignSelf: 'center' }}>
          {t('common:errors.reload')}
        </Button>
      </Stack>
    </Box>
  );
}

interface ErrorBoundaryProps {
  children: ReactNode;
}

interface ErrorBoundaryState {
  hasError: boolean;
}

// Catches render-time crashes anywhere in the app tree (a bug in a component, a bad prop, etc.)
// so the user sees an explanation and a way to recover instead of a blank white screen. This is
// a distinct failure mode from API errors (see apiError.ts) — there's no server request here, so
// no traceId to show; the fix is always "reload," so that's the only action offered.
export class ErrorBoundary extends Component<ErrorBoundaryProps, ErrorBoundaryState> {
  state: ErrorBoundaryState = { hasError: false };

  static getDerivedStateFromError(): ErrorBoundaryState {
    return { hasError: true };
  }

  componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    console.error('Unhandled error in component tree:', error, errorInfo);
  }

  render() {
    return this.state.hasError ? <ErrorFallback /> : this.props.children;
  }
}
