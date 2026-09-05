import type { FetchBaseQueryMeta } from '@reduxjs/toolkit/query/react';

export interface PaginationMeta {
  totalCount: number;
  pageNumber: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

/** Non-enumerable so it survives spreads/serialization checks without showing up in for..in or JSON.stringify. */
const PAGINATION_META = Symbol('paginationMeta');

export type PaginatedArray<T> = T[] & { [PAGINATION_META]?: PaginationMeta };

/**
 * RTK Query transformResponse for list endpoints backed by the backend's
 * PaginatedList<T> pattern. The response body stays a plain array (unchanged
 * contract, so every existing consumer of these hooks — dropdowns, dashboard
 * cards — keeps working untouched); pagination metadata from the X-Pagination
 * response header is attached as a hidden property instead of changing the
 * array's shape. Pages that want real pagination read it via getPaginationMeta.
 */
export function attachPaginationMeta<T>(response: T[], meta: FetchBaseQueryMeta | undefined): PaginatedArray<T> {
  const header = meta?.response?.headers.get('X-Pagination');
  const result = response as PaginatedArray<T>;
  if (header) {
    try {
      result[PAGINATION_META] = JSON.parse(header) as PaginationMeta;
    } catch {
      // Malformed header: fall through with no pagination metadata attached.
    }
  }
  return result;
}

export function getPaginationMeta<T>(data: PaginatedArray<T> | undefined): PaginationMeta | undefined {
  return data?.[PAGINATION_META];
}
