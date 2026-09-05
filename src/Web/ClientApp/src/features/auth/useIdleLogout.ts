import { useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { usePostApiAuthLogoutMutation } from '../../api/generated/apiSlice';
import { useAppDispatch } from '../../app/hooks';
import { useToast } from '../../components/feedback/ToastContext';
import { loggedOut } from './authSlice';

const IDLE_TIMEOUT_MS = 10 * 60 * 1000;
const ACTIVITY_EVENTS = ['mousedown', 'mousemove', 'keydown', 'wheel', 'scroll', 'touchstart'] as const;
// mousemove/scroll fire far more often than the timeout needs to be re-armed —
// only reset the timer when at least this long has passed since the last reset.
const RESET_THROTTLE_MS = 1000;

/** Mount once inside the authenticated app shell — logs the user out after IDLE_TIMEOUT_MS of no activity. */
export function useIdleLogout() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const toast = useToast();
  const { t } = useTranslation();
  const [logout] = usePostApiAuthLogoutMutation();

  const timeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const lastResetRef = useRef(0);

  useEffect(() => {
    const handleIdleTimeout = () => {
      void (async () => {
        try {
          await logout().unwrap();
        } finally {
          dispatch(loggedOut());
          toast.info(t('actions.sessionExpiredDueToInactivity'));
          navigate('/login', { replace: true });
        }
      })();
    };

    const resetTimer = () => {
      const now = Date.now();
      if (now - lastResetRef.current < RESET_THROTTLE_MS) return;
      lastResetRef.current = now;
      if (timeoutRef.current) clearTimeout(timeoutRef.current);
      timeoutRef.current = setTimeout(handleIdleTimeout, IDLE_TIMEOUT_MS);
    };

    resetTimer();
    ACTIVITY_EVENTS.forEach((event) => window.addEventListener(event, resetTimer, { passive: true }));

    return () => {
      if (timeoutRef.current) clearTimeout(timeoutRef.current);
      ACTIVITY_EVENTS.forEach((event) => window.removeEventListener(event, resetTimer));
    };
    // Intentionally runs once per mount (AppShell only mounts while authenticated) —
    // re-subscribing on every render would thrash the idle timer.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);
}
