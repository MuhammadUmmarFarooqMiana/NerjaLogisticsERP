import { useCallback, useEffect, useRef, useState } from 'react';
import { useAppDispatch, useAppSelector } from '../../app/hooks';
import { selectAccessToken, selectCurrentUser } from './authSlice';
import { pictureChanged, selectProfilePictureVersion } from './profilePictureSlice';

const PROFILE_PICTURE_URL = '/api/Employees/me/profile-picture';

/**
 * Loads the current user's profile picture, if any, through the [Authorize]-protected
 * endpoint (a plain <img src> can't carry the Bearer token) and exposes it as a short-lived
 * object URL — never a direct/public link. A 404 just means no picture has been uploaded
 * yet, so callers fall back to an initials avatar instead of treating it as an error.
 *
 * Every mounted instance of this hook (TopBar's avatar, MyProfilePage's avatar, ...) fetches
 * independently — there's no single cache entry for a binary blob to share via RTK Query —
 * so `refetch` dispatches a Redux-wide version bump instead of touching only local state,
 * refreshing every instance on screen, not just the one that triggered the upload/remove.
 */
export function useMyProfilePicture() {
  const dispatch = useAppDispatch();
  const accessToken = useAppSelector(selectAccessToken);
  const user = useAppSelector(selectCurrentUser);
  const version = useAppSelector(selectProfilePictureVersion);
  const [pictureUrl, setPictureUrl] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const objectUrlRef = useRef<string | null>(null);

  const revoke = useCallback(() => {
    if (objectUrlRef.current) {
      URL.revokeObjectURL(objectUrlRef.current);
      objectUrlRef.current = null;
    }
  }, []);

  useEffect(() => {
    if (!user) {
      revoke();
      setPictureUrl(null);
      setIsLoading(false);
      return;
    }

    let cancelled = false;
    setIsLoading(true);

    fetch(PROFILE_PICTURE_URL, {
      credentials: 'include',
      headers: accessToken ? { Authorization: `Bearer ${accessToken}` } : undefined,
    })
      .then((response) => (response.ok ? response.blob() : null))
      .then((blob) => {
        if (cancelled) return;
        revoke();
        if (blob) {
          const objectUrl = URL.createObjectURL(blob);
          objectUrlRef.current = objectUrl;
          setPictureUrl(objectUrl);
        } else {
          setPictureUrl(null);
        }
      })
      .catch(() => {
        if (!cancelled) setPictureUrl(null);
      })
      .finally(() => {
        if (!cancelled) setIsLoading(false);
      });

    return () => {
      cancelled = true;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [accessToken, user?.userId, version]);

  useEffect(() => () => revoke(), [revoke]);

  // Call after a successful upload/remove so the new (or absent) picture loads
  // immediately, everywhere it's shown — see the doc comment above.
  const refetch = useCallback(() => dispatch(pictureChanged()), [dispatch]);

  return { pictureUrl, isLoading, refetch };
}
