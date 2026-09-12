import { useState } from 'react';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import BadgeOutlinedIcon from '@mui/icons-material/BadgeOutlined';
import DescriptionOutlinedIcon from '@mui/icons-material/DescriptionOutlined';
import LocalPhoneOutlinedIcon from '@mui/icons-material/LocalPhoneOutlined';
import MailOutlineIcon from '@mui/icons-material/MailOutlined';
import PersonOutlineIcon from '@mui/icons-material/PersonOutlineOutlined';
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Grid,
  Paper,
  Stack,
  Tab,
  Tabs,
  TextField,
  Typography,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { usePostApiEmployeesByIdSubmitProfileMutation } from '../../api/employeesApi';
import { useGetApiEmployeeDocumentsQuery } from '../../api/employeeDocumentsApi';
import type { EmployeeDetailDto } from '../../api/generated/apiSlice';
import { useAppSelector } from '../../app/hooks';
import { FormDatePicker } from '../../components/shared/FormDatePicker';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { selectCurrentUser } from '../auth/authSlice';
import { useMyProfilePicture } from '../auth/useMyProfilePicture';
import { EmployeeDocumentsPanel } from './EmployeeDocumentsPanel';
import { ProfilePictureUploader } from './ProfilePictureUploader';
import { REQUIRED_EMPLOYEE_DOCUMENT_TYPES } from './employeeDocumentTypes';
import { buildProfileSubmissionSchema, type ProfileSubmissionFormValues } from './schemas';

interface ProfileSubmissionFormProps {
  /** Existing record when resubmitting after rejection — prefills previously entered values. */
  employee?: EmployeeDetailDto;
}

export default function ProfileSubmissionForm({ employee }: ProfileSubmissionFormProps) {
  const { t } = useTranslation('employees');
  const user = useAppSelector(selectCurrentUser);
  const toast = useToast();
  const [submitProfile, { isLoading }] = usePostApiEmployeesByIdSubmitProfileMutation();
  const { data: documents = [] } = useGetApiEmployeeDocumentsQuery();
  const { pictureUrl } = useMyProfilePicture();
  const [step, setStep] = useState<0 | 1>(0);

  const {
    control,
    register,
    handleSubmit,
    trigger,
    formState: { errors },
  } = useForm<ProfileSubmissionFormValues>({
    resolver: zodResolver(buildProfileSubmissionSchema(t)),
    defaultValues: {
      iqamaNumber: employee?.iqamaNumber ?? '',
      platformIdNumber: employee?.platformIdNumber ?? '',
      idExpiryDate: employee?.idExpiryDate ?? '',
      iqamaExpiryDate: employee?.iqamaExpiryDate ?? '',
      drivingLicenseExpiryDate: employee?.drivingLicenseExpiryDate ?? '',
      insuranceExpiryDate: employee?.insuranceExpiryDate ?? '',
    },
  });

  const handleNext = async () => {
    const valid = await trigger();
    if (valid) setStep(1);
  };

  // Tabs are freely clickable, so submitting from step 2 with invalid step-1
  // data would otherwise fail validation silently — those fields aren't even
  // mounted for the error text to appear next to. Snap back so it's visible.
  const onInvalid = () => setStep(0);

  const onSubmit = async (values: ProfileSubmissionFormValues) => {
    if (!user) return;

    // Mirrors the server-side check in SubmitProfileForReviewCommandHandler —
    // catching it here avoids a round trip and points the rider straight at
    // the Documents tab instead of a generic error after submitting. The profile picture
    // isn't an EmployeeDocument (see employeeDocumentTypes.ts), so it's checked separately
    // via whether an avatar has actually been uploaded, not via `documents`.
    const uploadedTypes = new Set(documents.map((d) => d.type));
    const missingLabels = REQUIRED_EMPLOYEE_DOCUMENT_TYPES
      .filter((docType) => !uploadedTypes.has(docType.enumName))
      .map((docType) => t(`profileSubmission.documentTypes.${docType.key}`));
    if (!pictureUrl) {
      missingLabels.unshift(t('profileSubmission.documentTypes.profilePicture'));
    }
    if (missingLabels.length > 0) {
      setStep(1);
      toast.error(t('documents.missingRequired', { types: missingLabels.join(', ') }));
      return;
    }

    try {
      await submitProfile({
        id: user.userId,
        submitProfileForReviewCommand: {
          userId: user.userId,
          iqamaNumber: values.iqamaNumber,
          platformIdNumber: values.platformIdNumber,
          idExpiryDate: values.idExpiryDate || undefined,
          iqamaExpiryDate: values.iqamaExpiryDate || undefined,
          drivingLicenseExpiryDate: values.drivingLicenseExpiryDate || undefined,
          insuranceExpiryDate: values.insuranceExpiryDate || undefined,
        },
      }).unwrap();
      toast.success(t('profileSubmission.success'));
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('profileSubmission.genericError')).join('\n'));
    }
  };

  return (
    <Box sx={{ width: '100%' }}>
      <Typography variant="h4" gutterBottom>
        {t('profileSubmission.title')}
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        {t('profileSubmission.subtitle')}
      </Typography>

      <Paper variant="outlined" sx={{ overflow: 'hidden' }}>
        <Tabs
          value={step}
          onChange={(_event, value: 0 | 1) => setStep(value)}
          sx={{ borderBottom: 1, borderColor: 'divider', px: 2 }}
        >
          <Tab
            value={0}
            label={t('profileSubmission.basicDetailsTab')}
            icon={<PersonOutlineIcon fontSize="small" />}
            iconPosition="start"
            sx={{ minHeight: 56 }}
          />
          <Tab
            value={1}
            label={t('profileSubmission.documentsTab')}
            icon={<DescriptionOutlinedIcon fontSize="small" />}
            iconPosition="start"
            sx={{ minHeight: 56 }}
          />
        </Tabs>

        <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit, onInvalid)(event)} noValidate sx={{ p: 3 }}>
          {step === 0 && (
            <Stack spacing={3}>
              <Alert severity="info">{t('profileSubmission.prefilledInfo')}</Alert>

              <Grid container spacing={2}>
                <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                  <TextField
                    label={t('profileSubmission.fullName')}
                    value={user?.fullName ?? ''}
                    fullWidth
                    disabled
                    slotProps={{ input: { startAdornment: <PersonOutlineIcon sx={{ mr: 1 }} color="action" /> } }}
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                  <TextField
                    label={t('profileSubmission.email')}
                    value={user?.email ?? ''}
                    fullWidth
                    disabled
                    slotProps={{ input: { startAdornment: <MailOutlineIcon sx={{ mr: 1 }} color="action" /> } }}
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                  <TextField
                    label={t('profileSubmission.phoneNumber')}
                    value={employee?.phoneNumber ?? ''}
                    fullWidth
                    disabled
                    slotProps={{ input: { startAdornment: <LocalPhoneOutlinedIcon sx={{ mr: 1 }} color="action" /> } }}
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                  <TextField
                    {...register('iqamaNumber')}
                    label={t('profileSubmission.iqamaNumber')}
                    required
                    fullWidth
                    error={!!errors.iqamaNumber}
                    helperText={errors.iqamaNumber?.message}
                    slotProps={{ input: { startAdornment: <BadgeOutlinedIcon sx={{ mr: 1 }} color="action" /> } }}
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                  <TextField
                    {...register('platformIdNumber')}
                    label={t('profileSubmission.platformIdNumber')}
                    required
                    fullWidth
                    error={!!errors.platformIdNumber}
                    helperText={errors.platformIdNumber?.message}
                    slotProps={{ input: { startAdornment: <BadgeOutlinedIcon sx={{ mr: 1 }} color="action" /> } }}
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                  <FormDatePicker
                    name="idExpiryDate"
                    control={control}
                    label={t('profileSubmission.idExpiryDate')}
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                  <FormDatePicker
                    name="iqamaExpiryDate"
                    control={control}
                    label={t('profileSubmission.iqamaExpiryDate')}
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                  <FormDatePicker
                    name="drivingLicenseExpiryDate"
                    control={control}
                    label={t('profileSubmission.drivingLicenseExpiryDate')}
                  />
                </Grid>
                <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                  <FormDatePicker
                    name="insuranceExpiryDate"
                    control={control}
                    label={t('profileSubmission.insuranceExpiryDate')}
                  />
                </Grid>
              </Grid>

              <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
                <Button variant="contained" onClick={() => void handleNext()}>
                  {t('profileSubmission.next')}
                </Button>
              </Box>
            </Stack>
          )}

          {step === 1 && (
            <Stack spacing={3}>
              <Alert severity="info">{t('profileSubmission.documentsInfo')}</Alert>

              <ProfilePictureUploader />

              <EmployeeDocumentsPanel />

              <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                <Button variant="text" onClick={() => setStep(0)}>
                  {t('profileSubmission.back')}
                </Button>
                <Button
                  type="submit"
                  variant="contained"
                  size="large"
                  disabled={isLoading}
                  startIcon={isLoading ? <CircularProgress size={18} color="inherit" /> : undefined}
                >
                  {t('profileSubmission.submit')}
                </Button>
              </Box>
            </Stack>
          )}
        </Box>
      </Paper>
    </Box>
  );
}
