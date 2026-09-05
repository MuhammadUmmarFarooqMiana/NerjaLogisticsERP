import { apiSlice } from './generated/apiSlice';
import { attachPaginationMeta } from '../lib/pagination';

export const finesApi = apiSlice.enhanceEndpoints({
  addTagTypes: ['Fine'],
  endpoints: {
    getApiFines: {
      providesTags: ['Fine'],
      transformResponse: attachPaginationMeta,
    },
    getApiFinesMe: {
      providesTags: ['Fine'],
      transformResponse: attachPaginationMeta,
    },
    postApiFines: {
      invalidatesTags: ['Fine'],
    },
  },
});

export const { useGetApiFinesQuery, useGetApiFinesMeQuery, usePostApiFinesMutation } = finesApi;
