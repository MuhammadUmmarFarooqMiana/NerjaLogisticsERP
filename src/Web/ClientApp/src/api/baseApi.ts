import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import type { BaseQueryFn, FetchArgs, FetchBaseQueryError } from '@reduxjs/toolkit/query/react';
import type { RootState } from '../app/store';
import { credentialsReceived, loggedOut } from '../features/auth/authSlice';

// Controller routes already include the "api/" prefix (ApiControllerBase's
// [Route("api/[controller]")]), and so does every path in the generated
// OpenAPI client — baseUrl stays empty to avoid double-prefixing to /api/api/....
const rawBaseQuery = fetchBaseQuery({
  baseUrl: '',
  credentials: 'include',
  prepareHeaders: (headers, { getState }) => {
    const token = (getState() as RootState).auth.accessToken;
    if (token) headers.set('Authorization', `Bearer ${token}`);
    return headers;
  },
});

interface RefreshResponse {
  accessToken: string;
  expiresAt: string;
}

// Dedupe concurrent 401s from multiple in-flight requests into a single
// refresh call, per RTK Query's standard re-auth pattern.
let refreshInFlight: Promise<boolean> | null = null;

const baseQueryWithReauth: BaseQueryFn<string | FetchArgs, unknown, FetchBaseQueryError> = async (
  args,
  api,
  extraOptions
) => {
  let result = await rawBaseQuery(args, api, extraOptions);

  if (result.error?.status === 401) {
    refreshInFlight ??= (async () => {
      const refreshResult = await rawBaseQuery(
        { url: '/api/auth/refreshtoken', method: 'POST' },
        api,
        extraOptions
      );

      if (refreshResult.data) {
        api.dispatch(credentialsReceived(refreshResult.data as RefreshResponse));
        return true;
      }

      api.dispatch(loggedOut());
      return false;
    })();

    const refreshed = await refreshInFlight;
    refreshInFlight = null;

    if (refreshed) {
      result = await rawBaseQuery(args, api, extraOptions);
    }
  }

  return result;
};

export const baseApi = createApi({
  reducerPath: 'api',
  baseQuery: baseQueryWithReauth,
  tagTypes: [],
  endpoints: () => ({}),
});
