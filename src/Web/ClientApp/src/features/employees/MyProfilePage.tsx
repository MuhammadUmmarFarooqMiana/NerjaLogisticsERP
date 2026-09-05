import { useEffect, useState } from 'react';
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Paper,
  Stack,
  Typography,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { useGetApiEmployeesMeQuery } from '../../api/employeesApi';
import { StatusBadge } from '../../components/shared/StatusBadge';
import { EmployeeDocumentsPanel } from './EmployeeDocumentsPanel';
import { EmployeeProfileFields } from './EmployeeProfileFields';
import { ProfilePictureUploader } from './ProfilePictureUploader';
import ProfileSubmissionForm from './ProfileSubmissionForm';

export default function MyProfilePage() {
  const { t } = useTranslation(['employees', 'auth']);
  const { data: employee, isLoading, error } = useGetApiEmployeesMeQuery();

  // Pops open once as soon as we learn the profile was rejected — the banner below
  // stays up as persistent context, but this is the attention-grabbing moment.
  const [rejectedDialogOpen, setRejectedDialogOpen] = useState(false);
  useEffect(() => {
    if (employee?.accountStatus === 'Rejected') {
      setRejectedDialogOpen(true);
    }
  }, [employee?.accountStatus]);

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error || !employee) {
    return <Alert severity="error">{t('myProfile.loadError')}</Alert>;
  }

  // Nothing to review yet, or a rejected submission — the form is still the right action.
  if (employee.accountStatus === 'Incomplete' || employee.accountStatus === 'Rejected') {
    return (
      <Box>
        <ProfilePictureUploader />
        {employee.accountStatus === 'Rejected' && employee.rejectionReason && (
          <>
            <Alert severity="warning" sx={{ mb: 3 }}>
              {t('myProfile.rejectedBanner', { reason: employee.rejectionReason })}
            </Alert>
            <Dialog open={rejectedDialogOpen} onClose={() => setRejectedDialogOpen(false)}>
              <DialogTitle>{t('auth:login.rejectedDialogTitle')}</DialogTitle>
              <DialogContent>
                <DialogContentText>
                  {t('auth:login.rejectedDialogBody', { reason: employee.rejectionReason })}
                </DialogContentText>
              </DialogContent>
              <DialogActions>
                <Button variant="contained" onClick={() => setRejectedDialogOpen(false)} autoFocus>
                  {t('auth:login.rejectedDialogContinue')}
                </Button>
              </DialogActions>
            </Dialog>
          </>
        )}
        <ProfileSubmissionForm employee={employee} />
      </Box>
    );
  }

  if (employee.accountStatus === 'PendingReview') {
    return (
      <Box>
        <ProfilePictureUploader />
        <Typography variant="h4" gutterBottom>
          {t('myProfile.title')}
        </Typography>
        <Alert severity="info" sx={{ maxWidth: 560 }}>
          {t('myProfile.pendingReview')}
        </Alert>
      </Box>
    );
  }

  // Active, Suspended, or Terminated — the profile has been through review; show it read-only.
  return (
    <Box>
      <Stack direction="row" sx={{ mb: 2, justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h4">{t('myProfile.title')}</Typography>
        <StatusBadge domain="accountStatus" status={employee.accountStatus ?? ''} />
      </Stack>
      <ProfilePictureUploader />
      <Paper variant="outlined" sx={{ p: 3, mb: 2 }}>
        <EmployeeProfileFields employee={employee} />
      </Paper>

      <Paper variant="outlined" sx={{ p: 3 }}>
        <Typography variant="h6" gutterBottom>
          {t('documents.sectionTitle')}
        </Typography>
        <EmployeeDocumentsPanel />
      </Paper>
    </Box>
  );
}
