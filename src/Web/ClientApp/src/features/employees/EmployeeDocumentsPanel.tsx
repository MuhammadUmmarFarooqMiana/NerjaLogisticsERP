import { useRef, useState } from 'react';
import CheckCircleOutlineIcon from '@mui/icons-material/CheckCircleOutlined';
import CloudUploadOutlinedIcon from '@mui/icons-material/CloudUploadOutlined';
import DownloadOutlinedIcon from '@mui/icons-material/DownloadOutlined';
import VisibilityOutlinedIcon from '@mui/icons-material/VisibilityOutlined';
import { Box, Button, Chip, CircularProgress, Grid, IconButton, Paper, Stack, Tooltip, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';
import {
  employeeDocumentDownloadUrl,
  useGetApiEmployeeDocumentsEmployeeByEmployeeIdQuery,
  useGetApiEmployeeDocumentsQuery,
  useUploadEmployeeDocumentMutation,
} from '../../api/employeeDocumentsApi';
import type { EmployeeDocumentDto } from '../../api/generated/apiSlice';
import { useAppSelector } from '../../app/hooks';
import { useToast } from '../../components/feedback/ToastContext';
import { getApiErrorMessages } from '../../lib/apiError';
import { downloadFile, previewFile } from '../../lib/downloadFile';
import { selectAccessToken } from '../auth/authSlice';
import { EMPLOYEE_DOCUMENT_TYPES } from './employeeDocumentTypes';

interface EmployeeDocumentsPanelProps {
  /** Omit to manage the signed-in user's own documents; pass to view/manage another employee's (Admin/Supervisor only). */
  employeeId?: string;
}

export function EmployeeDocumentsPanel({ employeeId }: EmployeeDocumentsPanelProps) {
  const { t } = useTranslation('employees');
  const toast = useToast();
  const accessToken = useAppSelector(selectAccessToken);
  const fileInputRefs = useRef<Record<number, HTMLInputElement | null>>({});
  const [uploadingType, setUploadingType] = useState<number | null>(null);
  const [previewingId, setPreviewingId] = useState<string | null>(null);

  const selfQuery = useGetApiEmployeeDocumentsQuery(undefined, { skip: !!employeeId });
  const employeeQuery = useGetApiEmployeeDocumentsEmployeeByEmployeeIdQuery(
    { employeeId: employeeId ?? '' },
    { skip: !employeeId }
  );
  const { data: documents = [] } = employeeId ? employeeQuery : selfQuery;
  const [upload] = useUploadEmployeeDocumentMutation();

  const docsByEnumName = new Map<string, EmployeeDocumentDto>();
  documents.forEach((d) => {
    if (d.type) docsByEnumName.set(d.type, d);
  });

  const handleFileSelected = async (typeValue: number, file: File | undefined) => {
    if (!file) return;
    setUploadingType(typeValue);
    try {
      await upload({ file, type: typeValue, employeeId }).unwrap();
      toast.success(t('documents.uploadSuccess'));
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('documents.genericError')).join('\n'));
    } finally {
      setUploadingType(null);
    }
  };

  const handleDownload = async (doc: EmployeeDocumentDto) => {
    if (!doc.id) return;
    try {
      await downloadFile(employeeDocumentDownloadUrl(doc.id), accessToken, doc.originalFileName ?? 'document');
    } catch {
      toast.error(t('documents.downloadError'));
    }
  };

  const handlePreview = async (doc: EmployeeDocumentDto) => {
    if (!doc.id) return;
    setPreviewingId(doc.id);
    try {
      await previewFile(employeeDocumentDownloadUrl(doc.id), accessToken);
    } catch {
      toast.error(t('documents.previewError'));
    } finally {
      setPreviewingId(null);
    }
  };

  return (
    <Grid container spacing={2}>
      {EMPLOYEE_DOCUMENT_TYPES.map(({ value, key, enumName, required }) => {
        const doc = docsByEnumName.get(enumName);
        const uploading = uploadingType === value;
        const missingRequired = required && !doc;

        return (
          <Grid key={key} size={{ xs: 12, sm: 6, md: 4 }}>
            <Paper
              variant="outlined"
              sx={{
                p: 2,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'space-between',
                gap: 1,
                ...(missingRequired && { borderColor: 'error.main' }),
              }}
            >
              <Stack direction="row" spacing={1.5} sx={{ alignItems: 'center', minWidth: 0 }}>
                {doc ? (
                  <CheckCircleOutlineIcon color="success" />
                ) : (
                  <CloudUploadOutlinedIcon color={missingRequired ? 'error' : 'disabled'} />
                )}
                <Box sx={{ minWidth: 0 }}>
                  <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                    <Typography variant="body2" noWrap>
                      {t(`profileSubmission.documentTypes.${key}`)}
                    </Typography>
                    {missingRequired && (
                      <Chip label={t('documents.required')} color="error" size="small" variant="outlined" />
                    )}
                  </Stack>
                  {doc && (
                    <Typography variant="caption" color="text.secondary" noWrap sx={{ display: 'block' }}>
                      {doc.originalFileName}
                    </Typography>
                  )}
                </Box>
              </Stack>

              <Stack direction="row" spacing={0.5} sx={{ flexShrink: 0 }}>
                {doc && (
                  <Tooltip title={t('documents.preview')}>
                    <span>
                      <IconButton size="small" disabled={previewingId === doc.id} onClick={() => void handlePreview(doc)}>
                        {previewingId === doc.id ? (
                          <CircularProgress size={16} />
                        ) : (
                          <VisibilityOutlinedIcon fontSize="small" />
                        )}
                      </IconButton>
                    </span>
                  </Tooltip>
                )}
                {doc && (
                  <Tooltip title={t('documents.download')}>
                    <IconButton size="small" onClick={() => void handleDownload(doc)}>
                      <DownloadOutlinedIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                )}
                <input
                  ref={(el) => {
                    fileInputRefs.current[value] = el;
                  }}
                  type="file"
                  hidden
                  onChange={(e) => void handleFileSelected(value, e.target.files?.[0])}
                />
                <Button
                  size="small"
                  variant="outlined"
                  disabled={uploading}
                  onClick={() => fileInputRefs.current[value]?.click()}
                  startIcon={uploading ? <CircularProgress size={16} /> : <CloudUploadOutlinedIcon fontSize="small" />}
                  sx={{ flexShrink: 0 }}
                >
                  {doc ? t('documents.replace') : t('documents.upload')}
                </Button>
              </Stack>
            </Paper>
          </Grid>
        );
      })}
    </Grid>
  );
}
