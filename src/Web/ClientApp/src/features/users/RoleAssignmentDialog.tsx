import { useEffect, useState } from 'react';
import {
  Button,
  Checkbox,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  FormControlLabel,
  FormGroup,
  DialogTitle,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { usePutApiUsersByIdRolesMutation } from '../../api/usersApi';
import type { UserListItemDto } from '../../api/generated/apiSlice';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { Roles } from '../../lib/roles';

interface RoleAssignmentDialogProps {
  open: boolean;
  user: UserListItemDto | null;
  onClose: () => void;
}

const ALL_ROLES = Object.values(Roles);

export function RoleAssignmentDialog({ open, user, onClose }: RoleAssignmentDialogProps) {
  const { t } = useTranslation('users');
  const toast = useToast();
  const [selected, setSelected] = useState<string[]>([]);
  const [updateRoles, { isLoading: saving }] = usePutApiUsersByIdRolesMutation();

  useEffect(() => {
    setSelected(user?.roles ?? []);
  }, [user]);

  const handleClose = () => {
    onClose();
  };

  const handleSave = async () => {
    if (!user?.id) return;
    try {
      await updateRoles({
        id: user.id,
        updateUserRolesCommand: { userId: user.id, roles: selected },
      }).unwrap();
      toast.success(t('roleDialog.success'));
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="xs" fullWidth>
      <DialogTitle>{t('roleDialog.title', { name: user?.fullName ?? user?.email })}</DialogTitle>
      <DialogContent>
        <DialogContentText sx={{ mb: 1 }}>{t('roleDialog.description')}</DialogContentText>
        <FormGroup>
          {ALL_ROLES.map((role) => (
            <FormControlLabel
              key={role}
              control={
                <Checkbox
                  checked={selected.includes(role)}
                  onChange={(event) => {
                    setSelected((current) =>
                      event.target.checked ? [...current, role] : current.filter((r) => r !== role)
                    );
                  }}
                />
              }
              label={t(`roles.${role}`)}
            />
          ))}
        </FormGroup>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>{t('common:actions.cancel')}</Button>
        <Button
          onClick={() => void handleSave()}
          variant="contained"
          disabled={saving || selected.length === 0}
          startIcon={saving ? <CircularProgress size={18} color="inherit" /> : undefined}
        >
          {t('roleDialog.submit')}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
