import { useState } from 'react';
import { Box, Button, Chip, Stack } from '@mui/material';
import type { ColumnDef } from '@tanstack/react-table';
import { useTranslation } from 'react-i18next';
import { useGetApiUsersQuery } from '../../api/usersApi';
import type { UserListItemDto } from '../../api/generated/apiSlice';
import { DataTable } from '../../components/shared/DataTable';
import { RoleAssignmentDialog } from './RoleAssignmentDialog';

export function UsersRolesView() {
  const { t } = useTranslation('users');
  const { data, isLoading, error } = useGetApiUsersQuery();
  const [target, setTarget] = useState<UserListItemDto | null>(null);

  const columns: ColumnDef<UserListItemDto, unknown>[] = [
    {
      accessorKey: 'fullName',
      header: t('list.columns.name'),
      cell: (info) => (info.getValue() as string | null) ?? '—',
    },
    { accessorKey: 'email', header: t('list.columns.email') },
    {
      accessorKey: 'roles',
      header: t('list.columns.roles'),
      cell: (info) => (
        <Stack direction="row" spacing={0.5} sx={{ flexWrap: 'wrap', rowGap: 0.5 }}>
          {(info.getValue() as string[]).map((role) => (
            <Chip key={role} size="small" label={t(`roles.${role}`)} />
          ))}
        </Stack>
      ),
    },
    {
      id: 'actions',
      header: '',
      cell: ({ row }) => (
        <Button
          size="small"
          onClick={(event) => {
            event.stopPropagation();
            setTarget(row.original);
          }}
        >
          {t('list.editRoles')}
        </Button>
      ),
    },
  ];

  return (
    <Box>
      <DataTable
        columns={columns}
        data={data ?? []}
        isLoading={isLoading}
        error={error}
        emptyMessage={t('list.empty')}
        enableGlobalFilter={false}
      />

      <RoleAssignmentDialog open={!!target} user={target} onClose={() => setTarget(null)} />
    </Box>
  );
}
