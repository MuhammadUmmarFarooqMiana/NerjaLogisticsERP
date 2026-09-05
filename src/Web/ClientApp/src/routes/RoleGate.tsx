import { Navigate, Outlet } from 'react-router-dom';
import { useAppSelector } from '../app/hooks';
import { selectCurrentUser } from '../features/auth/authSlice';

interface RoleGateProps {
  roles: string[];
}

// UX-only gate mirroring backend [Authorize(Roles = "...")] — hides/redirects
// away from UI the user isn't permitted to act on. The backend enforces the
// actual boundary regardless of what this component decides.
export default function RoleGate({ roles }: RoleGateProps) {
  const user = useAppSelector(selectCurrentUser);
  const allowed = !!user && roles.some((role) => user.roles.includes(role));

  if (!allowed) {
    return <Navigate to="/dashboard" replace />;
  }

  return <Outlet />;
}
