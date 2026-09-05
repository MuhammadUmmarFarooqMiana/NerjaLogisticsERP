import { Avatar, type SxProps, type Theme } from '@mui/material';
import { useAppSelector } from '../../app/hooks';
import { useMyProfilePicture } from '../../features/auth/useMyProfilePicture';
import { selectCurrentUser } from '../../features/auth/authSlice';
import { getInitials, stringToColor } from '../../lib/avatar';

interface UserAvatarProps {
  size?: number;
  sx?: SxProps<Theme>;
}

/** The current user's profile picture — fetched through the authorized endpoint, never a
 * direct link — falling back to an initials avatar when none has been uploaded. */
export function UserAvatar({ size = 36, sx }: UserAvatarProps) {
  const user = useAppSelector(selectCurrentUser);
  const { pictureUrl } = useMyProfilePicture();
  const fullName = user?.fullName ?? '';

  if (pictureUrl) {
    return (
      <Avatar
        src={pictureUrl}
        alt={fullName}
        sx={{ width: size, height: size, ...sx }}
      />
    );
  }

  return (
    <Avatar sx={{ width: size, height: size, bgcolor: stringToColor(fullName), fontSize: size * 0.4, ...sx }}>
      {getInitials(fullName)}
    </Avatar>
  );
}
