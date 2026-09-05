import { apiSlice } from './generated/apiSlice';
import { attachPaginationMeta } from '../lib/pagination';

// Adds cache tags on top of the generated Employees endpoints so
// Approve/Reject/SubmitProfile automatically refetch the list and detail
// views instead of leaving stale data on screen. Kept in a separate file
// since api/generated/apiSlice.ts is regenerated wholesale by the OpenAPI
// codegen and would lose any tags added directly to it.
export const employeesApi = apiSlice.enhanceEndpoints({
  addTagTypes: ['Employee'],
  endpoints: {
    deleteApiEmployeesMeProfilePicture: {
      invalidatesTags: ['Employee'],
    },
    getApiEmployees: {
      providesTags: ['Employee'],
      transformResponse: attachPaginationMeta,
    },
    getApiEmployeesMe: {
      providesTags: ['Employee'],
    },
    getApiEmployeesById: {
      providesTags: (_result, _error, arg) => [{ type: 'Employee' as const, id: arg.id }],
    },
    postApiEmployeesByIdApprove: {
      invalidatesTags: (_result, _error, arg) => [{ type: 'Employee' as const, id: arg.id }, 'Employee' as const],
    },
    postApiEmployeesByIdReject: {
      invalidatesTags: (_result, _error, arg) => [{ type: 'Employee' as const, id: arg.id }, 'Employee' as const],
    },
    postApiEmployeesByIdSubmitProfile: {
      invalidatesTags: ['Employee'],
    },
    postApiEmployees: {
      invalidatesTags: ['Employee'],
    },
    putApiEmployeesById: {
      invalidatesTags: (_result, _error, arg) => [{ type: 'Employee' as const, id: arg.id }, 'Employee' as const],
    },
    deleteApiEmployeesById: {
      invalidatesTags: ['Employee'],
    },
    postApiEmployeesByIdSuspend: {
      invalidatesTags: (_result, _error, arg) => [{ type: 'Employee' as const, id: arg.id }, 'Employee' as const],
    },
    postApiEmployeesByIdReactivate: {
      invalidatesTags: (_result, _error, arg) => [{ type: 'Employee' as const, id: arg.id }, 'Employee' as const],
    },
    postApiEmployeesByIdTerminate: {
      invalidatesTags: (_result, _error, arg) => [{ type: 'Employee' as const, id: arg.id }, 'Employee' as const],
    },
  },
}).injectEndpoints({
  endpoints: (build) => ({
    // Hand-written like companyDocumentsApi's uploadCompanyDocument — the generated
    // client can't express multipart/form-data (it just serializes `{ file }` as JSON),
    // so this builds the FormData itself.
    uploadMyProfilePicture: build.mutation<unknown, File>({
      query: (file) => {
        const formData = new FormData();
        formData.append('file', file);
        return {
          url: '/api/Employees/me/profile-picture',
          method: 'POST',
          body: formData,
        };
      },
      invalidatesTags: ['Employee'],
    }),
  }),
});

export const {
  useGetApiEmployeesQuery,
  useGetApiEmployeesByIdQuery,
  useGetApiEmployeesMeQuery,
  useGetApiEmployeesSupervisorsQuery,
  usePostApiEmployeesByIdApproveMutation,
  usePostApiEmployeesByIdRejectMutation,
  usePostApiEmployeesByIdSubmitProfileMutation,
  usePostApiEmployeesMutation,
  usePutApiEmployeesByIdMutation,
  useDeleteApiEmployeesByIdMutation,
  usePostApiEmployeesByIdSuspendMutation,
  usePostApiEmployeesByIdReactivateMutation,
  usePostApiEmployeesByIdTerminateMutation,
  useDeleteApiEmployeesMeProfilePictureMutation,
  useUploadMyProfilePictureMutation,
} = employeesApi;
