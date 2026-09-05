import { useEffect, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import AddIcon from '@mui/icons-material/Add';
import CreateNewFolderOutlinedIcon from '@mui/icons-material/CreateNewFolderOutlined';
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutlineOutlined';
import DownloadOutlinedIcon from '@mui/icons-material/DownloadOutlined';
import FolderOutlinedIcon from '@mui/icons-material/FolderOutlined';
import HomeOutlinedIcon from '@mui/icons-material/HomeOutlined';
import NavigateNextIcon from '@mui/icons-material/NavigateNext';
import VisibilityOutlinedIcon from '@mui/icons-material/VisibilityOutlined';
import {
  Box,
  Breadcrumbs,
  Button,
  CircularProgress,
  Grid,
  IconButton,
  Link,
  Paper,
  Stack,
  Tooltip,
  Typography,
} from '@mui/material';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import {
  companyDocumentDownloadUrl,
  useDeleteApiCompanyDocumentsFoldersByIdMutation,
  useDeleteCompanyDocumentMutation,
  useGetApiCompanyDocumentsFoldersQuery,
  useGetApiCompanyDocumentsQuery,
} from '../../api/companyDocumentsApi';
import type { CompanyDocumentDto, CompanyDocumentFolderDto } from '../../api/generated/apiSlice';
import { useAppSelector } from '../../app/hooks';
import { useToast } from '../../components/feedback/ToastContext';
import { ConfirmDialog } from '../../components/shared/ConfirmDialog';
import { DataTable } from '../../components/shared/DataTable';
import { downloadFile, previewFile } from '../../lib/downloadFile';
import { getApiErrorMessages } from '../../lib/apiError';
import { getPaginationMeta } from '../../lib/pagination';
import { Roles } from '../../lib/roles';
import { selectAccessToken, selectCurrentUser } from '../auth/authSlice';
import { CreateFolderDialog } from './CreateFolderDialog';
import { UploadCompanyDocumentDialog } from './UploadCompanyDocumentDialog';

interface BreadcrumbEntry {
  id: string;
  name: string;
}

interface CompanyDocumentsLocationState {
  path?: BreadcrumbEntry[];
}

export default function CompanyDocumentsPage() {
  const { t } = useTranslation('companyDocuments');
  const toast = useToast();
  const accessToken = useAppSelector(selectAccessToken);
  const user = useAppSelector(selectCurrentUser);
  const isAdmin = !!user?.roles.some((role) => role === Roles.Administrator);

  // The folder trail lives in browser history (via navigate's state) rather
  // than plain component state, so the browser's Back button steps out one
  // folder at a time instead of leaving the page entirely — it only lands on
  // Dashboard (or wherever) once every folder-level entry has been popped.
  const navigate = useNavigate();
  const location = useLocation();
  const path = (location.state as CompanyDocumentsLocationState | null)?.path ?? [];
  const currentFolderId = path.length > 0 ? path[path.length - 1].id : undefined;

  const goToPath = (nextPath: BreadcrumbEntry[]) => {
    navigate(location.pathname, { state: { path: nextPath } satisfies CompanyDocumentsLocationState });
  };

  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [createFolderOpen, setCreateFolderOpen] = useState(false);
  const [uploadOpen, setUploadOpen] = useState(false);
  const [deleteFolderTarget, setDeleteFolderTarget] = useState<CompanyDocumentFolderDto | null>(null);
  const [previewingId, setPreviewingId] = useState<string | null>(null);
  const [deleteDocTarget, setDeleteDocTarget] = useState<CompanyDocumentDto | null>(null);

  const { data: folders = [], isLoading: foldersLoading } = useGetApiCompanyDocumentsFoldersQuery({
    parentFolderId: currentFolderId,
  });
  const { data, isLoading, error } = useGetApiCompanyDocumentsQuery({
    folderId: currentFolderId,
    pageNumber: page + 1,
    pageSize,
  });
  const rowCount = getPaginationMeta(data)?.totalCount ?? 0;
  const [deleteFolder, { isLoading: deletingFolder }] = useDeleteApiCompanyDocumentsFoldersByIdMutation();
  const [deleteDocument, { isLoading: deletingDoc }] = useDeleteCompanyDocumentMutation();

  // Resets pagination whenever the viewed folder changes, regardless of
  // whether that came from clicking in (openFolder), a breadcrumb, or the
  // browser's Back/Forward buttons — none of the click handlers below fire
  // for that last case, so this can't live inline in them.
  useEffect(() => {
    setPage(0);
  }, [currentFolderId]);

  const openFolder = (folder: CompanyDocumentFolderDto) => {
    if (!folder.id) return;
    goToPath([...path, { id: folder.id, name: folder.name ?? '' }]);
  };

  const goToBreadcrumb = (index: number) => {
    // index -1 means the root itself.
    goToPath(path.slice(0, index + 1));
  };

  const handleDeleteFolder = async () => {
    if (!deleteFolderTarget?.id) return;
    try {
      await deleteFolder({ id: deleteFolderTarget.id }).unwrap();
      toast.success(t('deleteFolderSuccess'));
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    } finally {
      setDeleteFolderTarget(null);
    }
  };

  const handleDownload = async (doc: CompanyDocumentDto) => {
    if (!doc.id) return;
    try {
      await downloadFile(companyDocumentDownloadUrl(doc.id), accessToken, doc.originalFileName ?? 'document');
    } catch {
      toast.error(t('downloadError'));
    }
  };

  const handlePreview = async (doc: CompanyDocumentDto) => {
    if (!doc.id) return;
    setPreviewingId(doc.id);
    try {
      await previewFile(companyDocumentDownloadUrl(doc.id), accessToken);
    } catch {
      toast.error(t('previewError'));
    } finally {
      setPreviewingId(null);
    }
  };

  const handleDeleteDocument = async () => {
    if (!deleteDocTarget?.id) return;
    try {
      await deleteDocument({ id: deleteDocTarget.id }).unwrap();
      toast.success(t('deleteSuccess'));
    } catch (err) {
      toast.error(getApiErrorMessages(err, t('genericError')).join('\n'));
    } finally {
      setDeleteDocTarget(null);
    }
  };

  const columns: ColumnDef<CompanyDocumentDto, unknown>[] = [
    { accessorKey: 'title', header: t('columns.title') },
    {
      accessorKey: 'category',
      header: t('columns.category'),
      cell: (info) => t(`categories.${(info.getValue() as string).toLowerCase()}`),
    },
    { accessorKey: 'originalFileName', header: t('columns.fileName') },
    {
      accessorKey: 'uploadedByName',
      header: t('columns.uploadedBy'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
    {
      accessorKey: 'created',
      header: t('columns.uploaded'),
      cell: (info) => new Date(info.getValue() as string).toLocaleDateString(),
    },
    {
      id: 'actions',
      header: '',
      cell: ({ row }) => (
        <Stack direction="row" spacing={0.5}>
          <Tooltip title={t('preview')}>
            <span>
              <IconButton
                size="small"
                aria-label={t('preview')}
                disabled={previewingId === row.original.id}
                onClick={() => void handlePreview(row.original)}
              >
                {previewingId === row.original.id ? (
                  <CircularProgress size={16} />
                ) : (
                  <VisibilityOutlinedIcon fontSize="small" />
                )}
              </IconButton>
            </span>
          </Tooltip>
          <Tooltip title={t('download')}>
            <IconButton size="small" aria-label={t('download')} onClick={() => void handleDownload(row.original)}>
              <DownloadOutlinedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          {isAdmin && (
            <Tooltip title={t('delete')}>
              <IconButton size="small" aria-label={t('delete')} onClick={() => setDeleteDocTarget(row.original)}>
                <DeleteOutlineIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          )}
        </Stack>
      ),
    },
  ];

  return (
    <Box>
      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
        <Box>
          <Typography variant="h4" gutterBottom>
            {t('title')}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {t('subtitle')}
          </Typography>
        </Box>
        <Stack direction="row" spacing={1.5}>
          <Button variant="outlined" startIcon={<CreateNewFolderOutlinedIcon />} onClick={() => setCreateFolderOpen(true)}>
            {t('createFolder')}
          </Button>
          <Button variant="contained" startIcon={<AddIcon />} onClick={() => setUploadOpen(true)}>
            {t('uploadDocument')}
          </Button>
        </Stack>
      </Stack>

      <Breadcrumbs separator={<NavigateNextIcon fontSize="small" />} sx={{ mb: 2 }}>
        <Link
          component="button"
          type="button"
          underline={path.length === 0 ? 'none' : 'hover'}
          color={path.length === 0 ? 'text.primary' : 'inherit'}
          onClick={() => goToBreadcrumb(-1)}
          sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}
        >
          <HomeOutlinedIcon fontSize="small" />
          {t('root')}
        </Link>
        {path.map((entry, index) => (
          <Link
            key={entry.id}
            component="button"
            type="button"
            underline={index === path.length - 1 ? 'none' : 'hover'}
            color={index === path.length - 1 ? 'text.primary' : 'inherit'}
            onClick={() => goToBreadcrumb(index)}
          >
            {entry.name}
          </Link>
        ))}
      </Breadcrumbs>

      {!foldersLoading && folders.length > 0 && (
        <Grid container spacing={2} sx={{ mb: 3 }}>
          {folders.map((folder) => (
            <Grid key={folder.id} size={{ xs: 12, sm: 6, md: 4, lg: 3 }}>
              <Paper
                variant="outlined"
                sx={{
                  p: 2,
                  display: 'flex',
                  alignItems: 'center',
                  gap: 1,
                  cursor: 'pointer',
                  '&:hover': { bgcolor: 'action.hover' },
                }}
                onClick={() => openFolder(folder)}
              >
                <FolderOutlinedIcon color="action" />
                <Box sx={{ minWidth: 0, flex: 1 }}>
                  <Typography variant="body2" noWrap title={folder.name}>
                    {folder.name}
                  </Typography>
                  {folder.createdByName && (
                    <Typography variant="caption" color="text.secondary" noWrap sx={{ display: 'block' }}>
                      {t('createdBy', { name: folder.createdByName })}
                    </Typography>
                  )}
                </Box>
                {isAdmin && (
                  <IconButton
                    size="small"
                    aria-label={t('deleteFolder')}
                    onClick={(event) => {
                      event.stopPropagation();
                      setDeleteFolderTarget(folder);
                    }}
                  >
                    <DeleteOutlineIcon fontSize="small" />
                  </IconButton>
                )}
              </Paper>
            </Grid>
          ))}
        </Grid>
      )}

      <DataTable
        columns={columns}
        data={data ?? []}
        isLoading={isLoading}
        error={error}
        emptyMessage={t('empty')}
        serverPagination={{
          pageIndex: page,
          pageSize,
          rowCount,
          onPageChange: setPage,
          onPageSizeChange: (size) => {
            setPageSize(size);
            setPage(0);
          },
        }}
      />

      <CreateFolderDialog
        open={createFolderOpen}
        parentFolderId={currentFolderId}
        onClose={() => setCreateFolderOpen(false)}
      />

      <UploadCompanyDocumentDialog open={uploadOpen} folderId={currentFolderId} onClose={() => setUploadOpen(false)} />

      <ConfirmDialog
        open={!!deleteFolderTarget}
        title={t('deleteFolderConfirmTitle')}
        description={t('deleteFolderConfirmDescription', { name: deleteFolderTarget?.name })}
        destructive
        loading={deletingFolder}
        onConfirm={() => void handleDeleteFolder()}
        onCancel={() => setDeleteFolderTarget(null)}
      />

      <ConfirmDialog
        open={!!deleteDocTarget}
        title={t('deleteConfirmTitle')}
        description={t('deleteConfirmDescription', { name: deleteDocTarget?.title })}
        destructive
        loading={deletingDoc}
        onConfirm={() => void handleDeleteDocument()}
        onCancel={() => setDeleteDocTarget(null)}
      />
    </Box>
  );
}
