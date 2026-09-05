import { useState } from 'react';
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
import { useUploadCompanyDocumentMutation } from '../../api/companyDocumentsApi';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { COMPANY_DOCUMENT_CATEGORIES } from './companyDocumentCategories';

interface UploadCompanyDocumentDialogProps {
  open: boolean;
  /** Folder to upload into; undefined uploads to the root. */
  folderId?: string;
  onClose: () => void;
}

export function UploadCompanyDocumentDialog({ open, folderId, onClose }: UploadCompanyDocumentDialogProps) {
  const { t } = useTranslation('companyDocuments');
  const toast = useToast();
  const [upload, { isLoading }] = useUploadCompanyDocumentMutation();

  const [title, setTitle] = useState('');
  const [category, setCategory] = useState<number>(0);
  const [file, setFile] = useState<File | null>(null);
  const [touched, setTouched] = useState(false);

  const handleClose = () => {
    setTitle('');
    setCategory(0);
    setFile(null);
    setTouched(false);
    onClose();
  };

  const onSubmit = async () => {
    setTouched(true);
    if (!title.trim() || !file) return;
    try {
      await upload({ title: title.trim(), category, file, folderId }).unwrap();
      toast.success(t('uploadSuccess'));
      handleClose();
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    }
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{t('uploadDialog.title')}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          <TextField
            label={t('uploadDialog.documentTitle')}
            required
            fullWidth
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            error={touched && !title.trim()}
            helperText={touched && !title.trim() ? t('uploadDialog.titleRequired') : undefined}
          />
          <TextField
            select
            label={t('uploadDialog.category')}
            required
            fullWidth
            value={category}
            onChange={(e) => setCategory(Number(e.target.value))}
          >
            {COMPANY_DOCUMENT_CATEGORIES.map(({ value, key }) => (
              <MenuItem key={key} value={value}>
                {t(`categories.${key}`)}
              </MenuItem>
            ))}
          </TextField>
          <Button variant="outlined" component="label">
            {file ? file.name : t('uploadDialog.chooseFile')}
            <input
              type="file"
              hidden
              onChange={(e) => setFile(e.target.files?.[0] ?? null)}
            />
          </Button>
          {touched && !file && (
            <Box sx={{ color: 'error.main', fontSize: '0.75rem' }}>{t('uploadDialog.fileRequired')}</Box>
          )}
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>{t('common:actions.cancel')}</Button>
        <Button
          variant="contained"
          disabled={isLoading}
          startIcon={isLoading ? <CircularProgress size={18} color="inherit" /> : undefined}
          onClick={() => void onSubmit()}
        >
          {t('uploadDialog.submit')}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
