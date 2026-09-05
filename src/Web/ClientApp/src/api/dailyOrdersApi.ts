import { apiSlice } from './generated/apiSlice';
import { attachPaginationMeta } from '../lib/pagination';

// Adds cache tags on top of the generated DailyOrders endpoints — mirrors
// employeesApi.ts's pattern. Kept separate since generated/apiSlice.ts is
// regenerated wholesale by the OpenAPI codegen and would lose any tags added
// directly to it.
export const dailyOrdersApi = apiSlice
  .enhanceEndpoints({
    addTagTypes: ['DailyOrder'],
    endpoints: {
      getApiDailyOrdersMe: {
        providesTags: ['DailyOrder'],
      },
      getApiDailyOrders: {
        providesTags: ['DailyOrder'],
        transformResponse: attachPaginationMeta,
      },
      getApiDailyOrdersHistory: {
        providesTags: ['DailyOrder'],
        transformResponse: attachPaginationMeta,
      },
      postApiDailyOrdersUpsertOrderCount: {
        invalidatesTags: ['DailyOrder'],
      },
      postApiDailyOrdersClose: {
        invalidatesTags: ['DailyOrder'],
      },
      postApiDailyOrdersByIdApprove: {
        invalidatesTags: ['DailyOrder'],
      },
      postApiDailyOrdersByIdReject: {
        invalidatesTags: ['DailyOrder'],
        // The backend binds this as [FromBody] string, which needs a
        // JSON-encoded string ("foo", with quotes) — fetchBaseQuery only
        // auto-stringifies non-string bodies, so a bare string body here would
        // 415. Same fix as LeaveRequests' reject endpoint.
        query: (queryArg) => ({
          url: `/api/DailyOrders/${queryArg.id}/reject`,
          method: 'POST',
          body: JSON.stringify(queryArg.body),
          headers: { 'Content-Type': 'application/json' },
        }),
      },
      postApiDailyOrdersByIdCorrect: {
        invalidatesTags: ['DailyOrder'],
      },
    },
  })
  .injectEndpoints({
    endpoints: (build) => ({
      // Hand-written like companyDocumentsApi's deleteCompanyDocument — the
      // backend route exists (GetPendingDailyOrderApprovalsCountQuery /
      // DailyOrdersController.GetPendingApprovalsCount) but the generated
      // client hasn't been regenerated from a fresh OpenAPI spec yet.
      // Tagged 'DailyOrder' so it refetches the moment any daily order is
      // closed/approved/rejected/corrected anywhere in the app.
      getPendingDailyOrderApprovalsCount: build.query<number, void>({
        query: () => '/api/DailyOrders/pending-approvals-count',
        providesTags: ['DailyOrder'],
      }),
    }),
  });

export const {
  useGetApiDailyOrdersMeQuery,
  useGetApiDailyOrdersQuery,
  useGetApiDailyOrdersHistoryQuery,
  usePostApiDailyOrdersUpsertOrderCountMutation,
  usePostApiDailyOrdersCloseMutation,
  usePostApiDailyOrdersByIdApproveMutation,
  usePostApiDailyOrdersByIdRejectMutation,
  usePostApiDailyOrdersByIdCorrectMutation,
  useGetPendingDailyOrderApprovalsCountQuery,
} = dailyOrdersApi;
