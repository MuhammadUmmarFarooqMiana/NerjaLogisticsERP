import { configureStore, type Middleware } from '@reduxjs/toolkit';
import { baseApi } from '../api/baseApi';
import authReducer, { loggedOut } from '../features/auth/authSlice';
import profilePictureReducer from '../features/auth/profilePictureSlice';

// Without this, RTK Query keeps every cached response (e.g. the previous
// user's employee profile) in the store across a login/logout switch — the
// next user briefly sees stale data (or the wrong dialog) until their own
// queries happen to refetch. Logout is triggered from more than one place
// (TopBar, and baseApi's own re-auth wrapper on a failed refresh), so this is
// handled once, centrally, rather than duplicated at every call site.
const flushApiCacheOnLogout: Middleware = (storeApi) => (next) => (action) => {
  const result = next(action);
  if (loggedOut.match(action)) {
    storeApi.dispatch(baseApi.util.resetApiState());
  }
  return result;
};

export const store = configureStore({
  reducer: {
    auth: authReducer,
    profilePicture: profilePictureReducer,
    [baseApi.reducerPath]: baseApi.reducer,
  },
  middleware: (getDefaultMiddleware) => getDefaultMiddleware().concat(baseApi.middleware, flushApiCacheOnLogout),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
