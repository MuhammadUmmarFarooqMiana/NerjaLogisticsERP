import { apiSlice } from './generated/apiSlice';
import { attachPaginationMeta } from '../lib/pagination';

export const suppliersApi = apiSlice.enhanceEndpoints({
  addTagTypes: ['Supplier'],
  endpoints: {
    getApiSuppliers: {
      providesTags: ['Supplier'],
      transformResponse: attachPaginationMeta,
    },
    getApiSuppliersById: {
      providesTags: ['Supplier'],
    },
    postApiSuppliers: {
      invalidatesTags: ['Supplier'],
    },
    putApiSuppliersById: {
      invalidatesTags: ['Supplier'],
    },
    deleteApiSuppliersById: {
      invalidatesTags: ['Supplier'],
    },
  },
});

export const {
  useGetApiSuppliersQuery,
  useGetApiSuppliersByIdQuery,
  usePostApiSuppliersMutation,
  usePutApiSuppliersByIdMutation,
  useDeleteApiSuppliersByIdMutation,
} = suppliersApi;
