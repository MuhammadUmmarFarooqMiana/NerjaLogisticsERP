import { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import CancelIcon from '@mui/icons-material/Cancel';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutlineOutlined';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import { Alert, Box, Button, CircularProgress, Paper, Stack, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';
import {
  useDeleteApiEmployeesByIdMutation,
  useGetApiEmployeesByIdQuery,
  usePostApiEmployeesByIdApproveMutation,
  usePostApiEmployeesByIdReactivateMutation,
  usePostApiEmployeesByIdRejectMutation,
  usePostApiEmployeesByIdSuspendMutation,
  usePostApiEmployeesByIdTerminateMutation,
} from '../../api/employeesApi';
import { useAppSelector } from '../../app/hooks';
import { useToast } from '../../components/feedback/ToastContext';
import { ConfirmDialog } from '../../components/shared/ConfirmDialog';
import { StatusBadge } from '../../components/shared/StatusBadge';
import { getApiErrorMessages } from '../../lib/apiError';
import { Roles } from '../../lib/roles';
import { selectCurrentUser } from '../auth/authSlice';
import { ApproveEmployeeDialog, type ApproveEmployeeValues } from './ApproveEmployeeDialog';
import { EditEmployeeDialog } from './EditEmployeeDialog';
import { EmployeeDocumentsPanel } from './EmployeeDocumentsPanel';
import { EmployeeProfileFields } from './EmployeeProfileFields';
import { RejectEmployeeDialog } from './RejectEmployeeDialog';

export default function EmployeeDetailPage() {
  const { t } = useTranslation('employees');
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const toast = useToast();
  const user = useAppSelector(selectCurrentUser);
  const isAdmin = !!user?.roles.some((role) => role === Roles.Administrator);
  const { data: employee, isLoading, error } = useGetApiEmployeesByIdQuery({ id: id ?? '' }, { skip: !id });
  const [approve, { isLoading: approving }] = usePostApiEmployeesByIdApproveMutation();
  const [reject, { isLoading: rejecting }] = usePostApiEmployeesByIdRejectMutation();
  const [deleteEmployee, { isLoading: deleting }] = useDeleteApiEmployeesByIdMutation();
  const [suspend, { isLoading: suspending }] = usePostApiEmployeesByIdSuspendMutation();
  const [reactivate, { isLoading: reactivating }] = usePostApiEmployeesByIdReactivateMutation();
  const [terminate, { isLoading: terminating }] = usePostApiEmployeesByIdTerminateMutation();

  const [approveDialogOpen, setApproveDialogOpen] = useState(false);
  const [rejectDialogOpen, setRejectDialogOpen] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false);
  const [terminateConfirmOpen, setTerminateConfirmOpen] = useState(false);

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error || !employee) {
    return <Alert severity="error">{t('detail.genericError')}</Alert>;
  }

  const handleApprove = async (values: ApproveEmployeeValues) => {
    try {
      await approve({
        id: employee.id ?? '',
        approveEmployeeCommand: {
          platformId: values.platformId,
          vehicleId: values.vehicleId,
          supervisorId: values.supervisorId,
        },
      }).unwrap();
      setApproveDialogOpen(false);
      toast.success(t('detail.approveSuccess'));
    } catch {
      setApproveDialogOpen(false);
      toast.error(t('detail.genericError'));
    }
  };

  const handleReject = async (reason: string) => {
    try {
      await reject({ id: employee.id ?? '', rejectEmployeeCommand: { reason } }).unwrap();
      setRejectDialogOpen(false);
      toast.success(t('detail.rejectSuccess'));
    } catch {
      setRejectDialogOpen(false);
      toast.error(t('detail.genericError'));
    }
  };

  const handleDelete = async () => {
    try {
      await deleteEmployee({ id: employee.id ?? '' }).unwrap();
      setDeleteConfirmOpen(false);
      toast.success(t('admin.deleteSuccess'));
      navigate('/employees');
    } catch (err) {
      setDeleteConfirmOpen(false);
      toast.error(getApiErrorMessages(err, t('detail.genericError')).join('\n'));
    }
  };

  const handleSuspend = async () => {
    try {
      await suspend({ id: employee.id ?? '' }).unwrap();
      toast.success(t('admin.suspendSuccess'));
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('detail.genericError')).join('\n'));
    }
  };

  const handleReactivate = async () => {
    try {
      await reactivate({ id: employee.id ?? '' }).unwrap();
      toast.success(t('admin.reactivateSuccess'));
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('detail.genericError')).join('\n'));
    }
  };

  const handleTerminate = async () => {
    try {
      await terminate({ id: employee.id ?? '' }).unwrap();
      setTerminateConfirmOpen(false);
      toast.success(t('admin.terminateSuccess'));
    } catch (err) {
      setTerminateConfirmOpen(false);
      toast.error(getApiErrorMessages(err, t('detail.genericError')).join('\n'));
    }
  };

  const canReview = employee.accountStatus === 'PendingReview';

  return (
    <Box>
      <Stack direction="row" sx={{ mb: 2, justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h4">{employee.fullName}</Typography>
        <StatusBadge domain="accountStatus" status={employee.accountStatus ?? ''} />
      </Stack>

      <Paper variant="outlined" sx={{ p: 3, mb: 2 }}>
        <EmployeeProfileFields employee={employee} />
      </Paper>

      <Paper variant="outlined" sx={{ p: 3, mb: 2 }}>
        <Typography variant="h6" gutterBottom>
          {t('documents.sectionTitle')}
        </Typography>
        <EmployeeDocumentsPanel employeeId={employee.id} />
      </Paper>

      <Stack direction="row" spacing={2} sx={{ flexWrap: 'wrap', rowGap: 1 }}>
        <Button variant="text" startIcon={<ArrowBackIcon />} onClick={() => navigate(-1)}>
          {t('detail.backToList')}
        </Button>
        {canReview && (
          <>
            <Button
              variant="contained"
              color="success"
              startIcon={<CheckCircleIcon />}
              onClick={() => setApproveDialogOpen(true)}
            >
              {t('detail.approve')}
            </Button>
            <Button
              variant="contained"
              color="error"
              startIcon={<CancelIcon />}
              onClick={() => setRejectDialogOpen(true)}
            >
              {t('detail.reject')}
            </Button>
          </>
        )}
        {isAdmin && (
          <>
            <Button variant="outlined" startIcon={<EditOutlinedIcon />} onClick={() => setEditDialogOpen(true)}>
              {t('admin.edit')}
            </Button>
            {employee.accountStatus === 'Active' && (
              <Button variant="outlined" color="warning" disabled={suspending} onClick={() => void handleSuspend()}>
                {t('admin.suspend')}
              </Button>
            )}
            {employee.accountStatus === 'Suspended' && (
              <Button
                variant="outlined"
                color="success"
                disabled={reactivating}
                onClick={() => void handleReactivate()}
              >
                {t('admin.reactivate')}
              </Button>
            )}
            {employee.accountStatus !== 'Terminated' && employee.accountStatus !== 'Rejected' && (
              <Button variant="outlined" color="error" onClick={() => setTerminateConfirmOpen(true)}>
                {t('admin.terminate')}
              </Button>
            )}
            <Button
              variant="outlined"
              color="error"
              startIcon={<DeleteOutlineIcon />}
              onClick={() => setDeleteConfirmOpen(true)}
            >
              {t('admin.delete')}
            </Button>
          </>
        )}
      </Stack>

      <ApproveEmployeeDialog
        open={approveDialogOpen}
        loading={approving}
        onConfirm={(values) => void handleApprove(values)}
        onCancel={() => setApproveDialogOpen(false)}
      />

      <RejectEmployeeDialog
        open={rejectDialogOpen}
        loading={rejecting}
        onConfirm={(reason) => void handleReject(reason)}
        onCancel={() => setRejectDialogOpen(false)}
      />

      {isAdmin && (
        <>
          <EditEmployeeDialog open={editDialogOpen} employee={employee} onClose={() => setEditDialogOpen(false)} />

          <ConfirmDialog
            open={deleteConfirmOpen}
            title={t('admin.deleteConfirmTitle')}
            description={t('admin.deleteConfirmDescription', { name: employee.fullName })}
            destructive
            loading={deleting}
            onConfirm={() => void handleDelete()}
            onCancel={() => setDeleteConfirmOpen(false)}
          />

          <ConfirmDialog
            open={terminateConfirmOpen}
            title={t('admin.terminateConfirmTitle')}
            description={t('admin.terminateConfirmDescription', { name: employee.fullName })}
            destructive
            loading={terminating}
            onConfirm={() => void handleTerminate()}
            onCancel={() => setTerminateConfirmOpen(false)}
          />
        </>
      )}
    </Box>
  );
}
