import { Box, CircularProgress } from '@mui/material';
import { Navigate, Outlet } from 'react-router-dom';
import { useGetApiEmployeesMeQuery } from '../api/generated/apiSlice';
import { useAppSelector } from '../app/hooks';
import { selectCurrentUser } from '../features/auth/authSlice';
import { Roles } from '../lib/roles';

const PROFILE_PATH = '/profile';

// Staff roles run the platform rather than going through the rider profile
// approval workflow — never gated by their own Employee record's status.
const EXEMPT_ROLES: string[] = [Roles.Administrator, Roles.SoftwareEngineer];

// These statuses stick the user on the profile form — Incomplete/PendingReview
// because there's nothing to review yet, Rejected because they need to correct
// and resubmit. Suspended/Terminated are handled elsewhere (login popup,
// read-only profile view) — they don't bounce the user back to the form.
const RESTRICTED_STATUSES = new Set(['Incomplete', 'PendingReview', 'Rejected']);

// Wraps every route except /profile itself.
export default function RequireActiveProfile() {
  const user = useAppSelector(selectCurrentUser);
  const isExempt = !!user?.roles.some((role) => EXEMPT_ROLES.includes(role));

  const { data: employee, isLoading, isError } = useGetApiEmployeesMeQuery(undefined, { skip: isExempt });

  if (isExempt) {
    return <Outlet />;
  }

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  // Fail open on a transient fetch error rather than locking the user out of
  // the whole app over a network blip.
  if (isError || !employee) {
    return <Outlet />;
  }

  if (employee.accountStatus && RESTRICTED_STATUSES.has(employee.accountStatus)) {
    return <Navigate to={PROFILE_PATH} replace />;
  }

  return <Outlet />;
}
