import { apiSlice } from './generated/apiSlice';

export const usersApi = apiSlice.enhanceEndpoints({
  addTagTypes: ['User'],
  endpoints: {
    getApiUsers: {
      providesTags: ['User'],
    },
    putApiUsersByIdRoles: {
      invalidatesTags: ['User'],
    },
  },
});

export const { useGetApiUsersQuery, usePutApiUsersByIdRolesMutation } = usersApi;
