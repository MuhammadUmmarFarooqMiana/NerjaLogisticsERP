import { useState } from 'react';
import {
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  MenuItem,
  Stack,
  TextField,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { useGetApiPlatformsQuery, useGetApiVehiclesQuery } from '../../api/generated/apiSlice';
import { useGetApiEmployeesSupervisorsQuery } from '../../api/employeesApi';

export interface ApproveEmployeeValues {
  platformId?: string;
  vehicleId?: string;
  supervisorId?: string;
}

interface ApproveEmployeeDialogProps {
  open: boolean;
  loading?: boolean;
  onCancel: () => void;
  onConfirm: (values: ApproveEmployeeValues) => void;
}

export function ApproveEmployeeDialog({ open, loading = false, onCancel, onConfirm }: ApproveEmployeeDialogProps) {
  const { t } = useTranslation('employees');
  const [platformId, setPlatformId] = useState('');
  const [vehicleId, setVehicleId] = useState('');
  const [supervisorId, setSupervisorId] = useState('');

  const { data: platforms } = useGetApiPlatformsQuery(undefined, { skip: !open });
  const { data: vehicles } = useGetApiVehiclesQuery({ activeOnly: true }, { skip: !open });
  const { data: supervisors } = useGetApiEmployeesSupervisorsQuery(undefined, { skip: !open });

  const handleConfirm = () => {
    onConfirm({
      platformId: platformId || undefined,
      vehicleId: vehicleId || undefined,
      supervisorId: supervisorId || undefined,
    });
  };

  return (
    <Dialog open={open} onClose={onCancel} fullWidth maxWidth="sm">
      <DialogTitle>{t('detail.approveConfirmTitle')}</DialogTitle>
      <DialogContent>
        <DialogContentText sx={{ mb: 2 }}>{t('detail.approveConfirmDescription')}</DialogContentText>
        <Stack spacing={2}>
          <TextField
            select
            label={t('approveDialog.platform')}
            value={platformId}
            onChange={(event) => setPlatformId(event.target.value)}
            fullWidth
          >
            <MenuItem value="">{t('approveDialog.none')}</MenuItem>
            {platforms?.map((platform) => (
              <MenuItem key={platform.id} value={platform.id}>
                {platform.name}
              </MenuItem>
            ))}
          </TextField>
          <TextField
            select
            label={t('approveDialog.vehicle')}
            value={vehicleId}
            onChange={(event) => setVehicleId(event.target.value)}
            fullWidth
          >
            <MenuItem value="">{t('approveDialog.none')}</MenuItem>
            {vehicles?.map((vehicle) => (
              <MenuItem key={vehicle.id} value={vehicle.id}>
                {vehicle.registrationNumber}
                {vehicle.assignedEmployeeName ? ` — ${vehicle.assignedEmployeeName}` : ''}
              </MenuItem>
            ))}
          </TextField>
          <TextField
            select
            label={t('approveDialog.supervisor')}
            value={supervisorId}
            onChange={(event) => setSupervisorId(event.target.value)}
            fullWidth
          >
            <MenuItem value="">{t('approveDialog.none')}</MenuItem>
            {supervisors?.map((supervisor) => (
              <MenuItem key={supervisor.id} value={supervisor.id}>
                {supervisor.fullName}
              </MenuItem>
            ))}
          </TextField>
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onCancel} disabled={loading}>
          {t('common:actions.cancel')}
        </Button>
        <Button
          onClick={handleConfirm}
          color="success"
          variant="contained"
          disabled={loading}
          startIcon={loading ? <CircularProgress size={18} color="inherit" /> : undefined}
        >
          {t('detail.approve')}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
