import { useEffect } from 'react';
import { Controller, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import {
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  MenuItem,
  Stack,
  TextField,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { usePutApiVehiclesAllocationsByIdMutation } from '../../api/vehiclesApi';
import { useGetApiEmployeesQuery } from '../../api/employeesApi';
import type { AllocationRecordDto } from '../../api/generated/apiSlice';
import { FormDatePicker } from '../../components/shared/FormDatePicker';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { employeeOptionLabel } from '../../lib/employeeDisplay';
import { buildAllocateVehicleSchema, type AllocateVehicleFormValues } from './schemas';

interface EditAllocationDialogProps {
  open: boolean;
  allocation: AllocationRecordDto | null;
  onClose: () => void;
}

const emptyValues: AllocateVehicleFormValues = { employeeId: '', assignedDate: '' };

// Reallocation is Administrator-only on the backend — this dialog is only
// ever reachable from a page already gated to Administrator, so no extra
// role check is needed here.
export function EditAllocationDialog({ open, allocation, onClose }: EditAllocationDialogProps) {
  const { t } = useTranslation('vehicles');
  const toast = useToast();
  const { data: employees } = useGetApiEmployeesQuery({ status: 'Active' }, { skip: !open });
  const [updateAllocation, { isLoading: saving }] = usePutApiVehiclesAllocationsByIdMutation();

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<AllocateVehicleFormValues>({
    resolver: zodResolver(buildAllocateVehicleSchema(t)),
    defaultValues: emptyValues,
  });

  useEffect(() => {
    if (open) {
      reset(
        allocation
          ? { employeeId: allocation.employeeId, assignedDate: allocation.assignedDate }
          : emptyValues
      );
    }
  }, [open, allocation, reset]);

  const handleClose = () => {
    reset(emptyValues);
    onClose();
  };

  const onSubmit = async (values: AllocateVehicleFormValues) => {
    if (!allocation?.id) return;
    try {
      await updateAllocation({
        id: allocation.id,
        updateVehicleAllocationCommand: { id: allocation.id, ...values },
      }).unwrap();
      toast.success(t('reallocateSuccess'));
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{t('editAllocationDialog.title')}</DialogTitle>
      <Box component="form" onSubmit={(event) => void handleSubmit(onSubmit)(event)} noValidate>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            <Controller
              name="employeeId"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  select
                  label={t('allocateDialog.employee')}
                  required
                  fullWidth
                  error={!!errors.employeeId}
                  helperText={errors.employeeId?.message}
                >
                  {(employees ?? []).map((employee) => (
                    <MenuItem key={employee.id} value={employee.id}>
                      {employeeOptionLabel(employee)}
                    </MenuItem>
                  ))}
                </TextField>
              )}
            />
            <FormDatePicker
              name="assignedDate"
              control={control}
              label={t('allocateDialog.assignedDate')}
              required
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>{t('common:actions.cancel')}</Button>
          <Button
            type="submit"
            variant="contained"
            disabled={saving}
            startIcon={saving ? <CircularProgress size={18} color="inherit" /> : undefined}
          >
            {t('common:actions.confirm')}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
