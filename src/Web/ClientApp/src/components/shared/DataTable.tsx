import { useEffect, useState } from 'react';
import {
  flexRender,
  getCoreRowModel,
  getFilteredRowModel,
  getPaginationRowModel,
  getSortedRowModel,
  useReactTable,
  type ColumnDef,
  type SortingState,
} from '@tanstack/react-table';
import {
  Alert,
  Box,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  TableSortLabel,
  TextField,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { getApiErrorMessages } from '../../lib/apiError';

/** Drives the table from a backend PaginatedList<T> endpoint instead of paginating `data` client-side. */
export interface ServerPaginationProps {
  pageIndex: number;
  pageSize: number;
  /** Total row count across every page (from the X-Pagination response header — see getPaginationMeta). */
  rowCount: number;
  onPageChange: (pageIndex: number) => void;
  onPageSizeChange: (pageSize: number) => void;
}

interface DataTableProps<T> {
  columns: ColumnDef<T, unknown>[];
  data: T[];
  isLoading?: boolean;
  emptyMessage?: string;
  /**
   * The `error` RTK Query returned for the request that produced `data`. Shown as a banner above
   * the table (via getApiErrorMessages) whenever set. This is deliberately separate from the
   * empty-state message: RTK Query keeps the last successful `data` across arg changes to avoid
   * UI flicker, so a filter change that fails to refetch would otherwise leave stale rows on
   * screen with no indication the refresh actually failed.
   */
  error?: unknown;
  /** Set false for small, pre-filtered lists that don't need a search box. */
  enableGlobalFilter?: boolean;
  /** Navigates to a detail view, etc. Omit for tables that are display-only. */
  onRowClick?: (row: T) => void;
  /**
   * When set, `data` is assumed to already be just the current page (fetched with
   * pageNumber/pageSize against a paginated backend endpoint) — the table stops
   * slicing/counting client-side and drives the pager from these values instead.
   * The search box is hidden in this mode: filtering only the loaded page would
   * silently miss matches on every other page, which is worse than no search at all.
   */
  serverPagination?: ServerPaginationProps;
}

export const ROWS_PER_PAGE_OPTIONS = [10, 25, 50, 100, 500];

export function DataTable<T>({
  columns,
  data,
  isLoading = false,
  emptyMessage,
  error,
  enableGlobalFilter = true,
  onRowClick,
  serverPagination,
}: DataTableProps<T>) {
  const { t } = useTranslation();
  const [sorting, setSorting] = useState<SortingState>([]);
  const [globalFilter, setGlobalFilter] = useState('');
  const [clientPagination, setClientPagination] = useState({ pageIndex: 0, pageSize: 10 });

  const pagination = serverPagination
    ? { pageIndex: serverPagination.pageIndex, pageSize: serverPagination.pageSize }
    : clientPagination;

  const table = useReactTable({
    data,
    columns,
    manualPagination: !!serverPagination,
    pageCount: serverPagination
      ? Math.max(1, Math.ceil(serverPagination.rowCount / serverPagination.pageSize))
      : undefined,
    state: { sorting, globalFilter, pagination },
    onSortingChange: setSorting,
    onGlobalFilterChange: setGlobalFilter,
    onPaginationChange: setClientPagination,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
  });

  const rows = table.getRowModel().rows;
  const filteredCount = serverPagination ? serverPagination.rowCount : table.getFilteredRowModel().rows.length;
  const showSearch = enableGlobalFilter && !serverPagination;

  // Filtering, an upstream data refresh (create/delete/role-scope change), or
  // a rows-per-page change can all shrink the result set out from under the
  // current page — without this, the user lands on a blank page instead of
  // being bounced back to the last real one. Search always jumps to page 1
  // (handled in the TextField's onChange below) since that's what users expect
  // from a fresh query; this effect is the general safety net for every other case.
  // Server-paginated tables don't need this: the caller owns pageIndex and is
  // responsible for clamping it against the row count it already has.
  useEffect(() => {
    if (serverPagination) return;
    const pageCount = Math.max(1, Math.ceil(filteredCount / clientPagination.pageSize));
    if (clientPagination.pageIndex > pageCount - 1) {
      setClientPagination((current) => ({ ...current, pageIndex: pageCount - 1 }));
    }
  }, [filteredCount, clientPagination.pageIndex, clientPagination.pageSize, serverPagination]);

  return (
    <Paper variant="outlined">
      {showSearch && (
        <Box sx={{ p: 2 }}>
          <TextField
            size="small"
            label={t('table.search')}
            value={globalFilter}
            onChange={(event) => {
              setGlobalFilter(event.target.value);
              // Every new search starts from page 1 — otherwise the user could stay
              // on, say, page 3 of a totally different result set with no indication why.
              setClientPagination((current) => ({ ...current, pageIndex: 0 }));
            }}
            fullWidth
          />
        </Box>
      )}
      {!!error && (
        <Box sx={{ px: 2, pt: showSearch ? 0 : 2 }}>
          <Alert severity="error" sx={{ whiteSpace: 'pre-line' }}>
            {getApiErrorMessages(error, t('table.loadError')).join('\n')}
          </Alert>
        </Box>
      )}
      <TableContainer>
        <Table>
          <TableHead>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id}>
                {headerGroup.headers.map((header) => (
                  <TableCell key={header.id}>
                    {header.isPlaceholder ? null : header.column.getCanSort() ? (
                      <TableSortLabel
                        active={!!header.column.getIsSorted()}
                        direction={header.column.getIsSorted() || undefined}
                        onClick={header.column.getToggleSortingHandler()}
                      >
                        {flexRender(header.column.columnDef.header, header.getContext())}
                      </TableSortLabel>
                    ) : (
                      flexRender(header.column.columnDef.header, header.getContext())
                    )}
                  </TableCell>
                ))}
              </TableRow>
            ))}
          </TableHead>
          <TableBody>
            {rows.length === 0 && (
              <TableRow>
                <TableCell colSpan={columns.length} align="center">
                  {isLoading ? t('actions.loading') : (emptyMessage ?? t('table.empty'))}
                </TableCell>
              </TableRow>
            )}
            {rows.map((row) => (
              <TableRow
                key={row.id}
                hover
                onClick={onRowClick ? () => onRowClick(row.original) : undefined}
                sx={onRowClick ? { cursor: 'pointer' } : undefined}
              >
                {row.getVisibleCells().map((cell) => (
                  <TableCell key={cell.id}>{flexRender(cell.column.columnDef.cell, cell.getContext())}</TableCell>
                ))}
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
      <TablePagination
        component="div"
        count={filteredCount}
        page={pagination.pageIndex}
        rowsPerPage={pagination.pageSize}
        rowsPerPageOptions={ROWS_PER_PAGE_OPTIONS}
        labelRowsPerPage={t('table.rowsPerPage')}
        labelDisplayedRows={({ from, to, count }) => t('table.displayedRows', { from, to, count })}
        onPageChange={(_event, page) => {
          if (serverPagination) serverPagination.onPageChange(page);
          else table.setPageIndex(page);
        }}
        onRowsPerPageChange={(event) => {
          const size = Number(event.target.value);
          if (serverPagination) serverPagination.onPageSizeChange(size);
          else table.setPageSize(size);
        }}
      />
    </Paper>
  );
}
