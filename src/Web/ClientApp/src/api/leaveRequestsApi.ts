import { apiSlice } from './generated/apiSlice';
import { attachPaginationMeta } from '../lib/pagination';

export const leaveRequestsApi = apiSlice.enhanceEndpoints({
  addTagTypes: ['LeaveRequest'],
  endpoints: {
    getApiLeaveRequestsMe: {
      providesTags: ['LeaveRequest'],
      transformResponse: attachPaginationMeta,
    },
    getApiLeaveRequests: {
      providesTags: ['LeaveRequest'],
      transformResponse: attachPaginationMeta,
    },
    postApiLeaveRequests: {
      invalidatesTags: ['LeaveRequest'],
    },
    postApiLeaveRequestsByIdApprove: {
      invalidatesTags: ['LeaveRequest'],
    },
    postApiLeaveRequestsByIdReject: {
      invalidatesTags: ['LeaveRequest'],
      // The backend binds this as [FromBody] string, which needs a
      // JSON-encoded string ("foo", with quotes) — fetchBaseQuery only
      // auto-stringifies non-string bodies, so a bare string body here would
      // 415. Force explicit JSON encoding + content-type instead (same fix
      // as MarkAsPaid in monthlySummariesApi.ts).
      query: (queryArg) => ({
        url: `/api/LeaveRequests/${queryArg.id}/reject`,
        method: 'POST',
        body: JSON.stringify(queryArg.body),
        headers: { 'Content-Type': 'application/json' },
      }),
    },
  },
});

export const {
  useGetApiLeaveRequestsMeQuery,
  useGetApiLeaveRequestsQuery,
  usePostApiLeaveRequestsMutation,
  usePostApiLeaveRequestsByIdApproveMutation,
  usePostApiLeaveRequestsByIdRejectMutation,
} = leaveRequestsApi;
