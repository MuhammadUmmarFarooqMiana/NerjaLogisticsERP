import { Box, CircularProgress } from '@mui/material';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAppSelector } from '../app/hooks';
import { selectAuthStatus } from '../features/auth/authSlice';

// Redirects to /login if the session-restore bootstrap (App.tsx) determined
// there's no valid refresh cookie. This is a UX convenience only — the
// backend's [Authorize] attributes are the real security boundary.
export default function ProtectedRoute() {
  const status = useAppSelector(selectAuthStatus);
  const location = useLocation();

  if (status === 'idle') {
    return (
      <Box sx={{ display: 'flex', minHeight: '100vh', alignItems: 'center', justifyContent: 'center' }}>
        <CircularProgress />
      </Box>
    );
  }

  if (status === 'unauthenticated') {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />;
  }

  return <Outlet />;
}
