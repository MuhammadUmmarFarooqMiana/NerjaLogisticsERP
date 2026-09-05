import { apiSlice } from './generated/apiSlice';
import { attachPaginationMeta } from '../lib/pagination';

export const advancesApi = apiSlice.enhanceEndpoints({
  addTagTypes: ['Advance'],
  endpoints: {
    getApiAdvances: {
      providesTags: ['Advance'],
      transformResponse: attachPaginationMeta,
    },
    getApiAdvancesMe: {
      providesTags: ['Advance'],
      transformResponse: attachPaginationMeta,
    },
    postApiAdvances: {
      invalidatesTags: ['Advance'],
    },
  },
});

export const {
  useGetApiAdvancesQuery,
  useGetApiAdvancesMeQuery,
  usePostApiAdvancesMutation,
} = advancesApi;
