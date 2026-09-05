import { Box, Button, CircularProgress, Stack, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';
import {
  useDeleteApiEmployeesMeProfilePictureMutation,
  useUploadMyProfilePictureMutation,
} from '../../api/employeesApi';
import { UserAvatar } from '../../components/layout/UserAvatar';
import { useToast } from '../../components/feedback/ToastContext';
import { useMyProfilePicture } from '../auth/useMyProfilePicture';

/** Header block for MyProfilePage — lets a user set/replace/remove their own photo.
 * Shown regardless of account-review status, since a photo can be set at any time. */
export function ProfilePictureUploader() {
  const { t } = useTranslation('employees');
  const toast = useToast();
  const { pictureUrl, refetch } = useMyProfilePicture();
  const [upload, { isLoading: isUploading }] = useUploadMyProfilePictureMutation();
  const [remove, { isLoading: isRemoving }] = useDeleteApiEmployeesMeProfilePictureMutation();

  const handleFileChange = async (file: File | undefined) => {
    if (!file) return;
    try {
      await upload(file).unwrap();
      refetch();
      toast.success(t('myProfile.photoUploadSuccess'));
    } catch {
      toast.error(t('myProfile.photoUploadError'));
    }
  };

  const handleRemove = async () => {
    try {
      await remove().unwrap();
      refetch();
      toast.success(t('myProfile.photoRemoveSuccess'));
    } catch {
      toast.error(t('myProfile.photoRemoveError'));
    }
  };

  const isBusy = isUploading || isRemoving;

  return (
    <Stack direction="row" spacing={2} sx={{ alignItems: 'center', mb: 3 }}>
      <UserAvatar size={72} />
      <Box>
        <Stack direction="row" spacing={1}>
          <Button variant="outlined" size="small" component="label" disabled={isBusy}>
            {isUploading ? <CircularProgress size={16} sx={{ mr: 1 }} /> : null}
            {t('myProfile.changePhoto')}
            <input
              type="file"
              hidden
              accept="image/jpeg,image/png"
              onChange={(e) => void handleFileChange(e.target.files?.[0])}
            />
          </Button>
          {pictureUrl && (
            <Button variant="text" size="small" color="error" disabled={isBusy} onClick={() => void handleRemove()}>
              {isRemoving ? <CircularProgress size={16} sx={{ mr: 1 }} /> : null}
              {t('myProfile.removePhoto')}
            </Button>
          )}
        </Stack>
        <Typography variant="caption" color="text.secondary">
          {t('myProfile.photoHint')}
        </Typography>
      </Box>
    </Stack>
  );
}
