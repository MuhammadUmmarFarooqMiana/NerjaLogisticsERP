import { apiSlice } from './generated/apiSlice';
import { attachPaginationMeta } from '../lib/pagination';

export const mechanicsApi = apiSlice.enhanceEndpoints({
  addTagTypes: ['Mechanic'],
  endpoints: {
    getApiMechanics: {
      providesTags: ['Mechanic'],
      transformResponse: attachPaginationMeta,
    },
    getApiMechanicsById: {
      providesTags: ['Mechanic'],
    },
    postApiMechanics: {
      invalidatesTags: ['Mechanic'],
    },
    putApiMechanicsById: {
      invalidatesTags: ['Mechanic'],
    },
    deleteApiMechanicsById: {
      invalidatesTags: ['Mechanic'],
    },
  },
});

export const {
  useGetApiMechanicsQuery,
  useGetApiMechanicsByIdQuery,
  usePostApiMechanicsMutation,
  usePutApiMechanicsByIdMutation,
  useDeleteApiMechanicsByIdMutation,
} = mechanicsApi;
