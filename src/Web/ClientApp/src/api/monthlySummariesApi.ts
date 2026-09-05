import { apiSlice } from './generated/apiSlice';
import { attachPaginationMeta } from '../lib/pagination';

export const monthlySummariesApi = apiSlice.enhanceEndpoints({
  addTagTypes: ['MonthlySummary'],
  endpoints: {
    getApiMonthlySummaries: {
      providesTags: ['MonthlySummary'],
      transformResponse: attachPaginationMeta,
    },
    getApiMonthlySummariesById: {
      providesTags: ['MonthlySummary'],
    },
    getApiMonthlySummariesMe: {
      providesTags: ['MonthlySummary'],
      transformResponse: attachPaginationMeta,
    },
    postApiMonthlySummariesGenerate: {
      invalidatesTags: ['MonthlySummary'],
    },
    postApiMonthlySummariesByIdVerify: {
      invalidatesTags: ['MonthlySummary'],
    },
    postApiMonthlySummariesByIdMarkAsPaid: {
      invalidatesTags: ['MonthlySummary'],
      // The backend binds this as [FromBody] string?, which needs a
      // JSON-encoded string ("foo", with quotes) — fetchBaseQuery only
      // auto-stringifies non-string bodies, so a bare string body here would
      // 415. Force explicit JSON encoding + content-type instead.
      query: (queryArg) => ({
        url: `/api/MonthlySummaries/${queryArg.id}/MarkAsPaid`,
        method: 'POST',
        body: JSON.stringify(queryArg.body),
        headers: { 'Content-Type': 'application/json' },
      }),
    },
  },
});

export const {
  useGetApiMonthlySummariesQuery,
  useGetApiMonthlySummariesByIdQuery,
  useGetApiMonthlySummariesMeQuery,
  usePostApiMonthlySummariesGenerateMutation,
  usePostApiMonthlySummariesByIdVerifyMutation,
  usePostApiMonthlySummariesByIdMarkAsPaidMutation,
} = monthlySummariesApi;
