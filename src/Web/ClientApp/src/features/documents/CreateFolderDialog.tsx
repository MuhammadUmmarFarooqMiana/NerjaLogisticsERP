import { useState } from 'react';
import {
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  TextField,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { usePostApiCompanyDocumentsFoldersMutation } from '../../api/companyDocumentsApi';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';

interface CreateFolderDialogProps {
  open: boolean;
  /** Folder to create the new one inside; undefined creates at the root. */
  parentFolderId?: string;
  onClose: () => void;
}

export function CreateFolderDialog({ open, parentFolderId, onClose }: CreateFolderDialogProps) {
  const { t } = useTranslation('companyDocuments');
  const toast = useToast();
  const [createFolder, { isLoading }] = usePostApiCompanyDocumentsFoldersMutation();

  const [name, setName] = useState('');
  const [touched, setTouched] = useState(false);

  const handleClose = () => {
    setName('');
    setTouched(false);
    onClose();
  };

  const onSubmit = async () => {
    setTouched(true);
    if (!name.trim()) return;
    try {
      await createFolder({
        createCompanyDocumentFolderCommand: { name: name.trim(), parentFolderId },
      }).unwrap();
      toast.success(t('createFolderSuccess'));
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{t('createFolderDialog.title')}</DialogTitle>
      <Box component="form" onSubmit={(event) => { event.preventDefault(); void onSubmit(); }} noValidate>
        <DialogContent>
          <TextField
            autoFocus
            label={t('createFolderDialog.folderName')}
            required
            fullWidth
            sx={{ mt: 1 }}
            value={name}
            onChange={(e) => setName(e.target.value)}
            error={touched && !name.trim()}
            helperText={touched && !name.trim() ? t('createFolderDialog.nameRequired') : undefined}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>{t('common:actions.cancel')}</Button>
          <Button
            type="submit"
            variant="contained"
            disabled={isLoading}
            startIcon={isLoading ? <CircularProgress size={18} color="inherit" /> : undefined}
          >
            {t('createFolderDialog.submit')}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
