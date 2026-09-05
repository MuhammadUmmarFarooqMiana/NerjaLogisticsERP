import { createAsyncThunk, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { decodeAccessToken } from '../../lib/jwt';
import type { RootState } from '../../app/store';

export interface AuthUser {
  userId: string;
  email: string;
  fullName: string;
  roles: string[];
}

export type AuthStatus = 'idle' | 'authenticated' | 'unauthenticated';

interface AuthState {
  accessToken: string | null;
  accessTokenExpiresAt: string | null;
  user: AuthUser | null;
  status: AuthStatus;
}

const initialState: AuthState = {
  accessToken: null,
  accessTokenExpiresAt: null,
  user: null,
  status: 'idle',
};

interface RefreshResponse {
  accessToken: string;
  expiresAt: string;
}

// Silent session restore on app load: the refresh token lives in an httpOnly
// cookie, sent automatically — this either yields a fresh access token or
// fails quietly, so plain fetch is used instead of RTK Query to avoid a
// circular import with baseApi (which itself dispatches auth actions).
export const bootstrapAuth = createAsyncThunk<RefreshResponse>('auth/bootstrap', async (_, { rejectWithValue }) => {
  const response = await fetch('/api/auth/refreshtoken', { method: 'POST', credentials: 'include' });
  if (!response.ok) return rejectWithValue(null);
  return (await response.json()) as RefreshResponse;
});

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    credentialsReceived(state, action: PayloadAction<RefreshResponse>) {
      state.accessToken = action.payload.accessToken;
      state.accessTokenExpiresAt = action.payload.expiresAt;
      state.user = decodeAccessToken(action.payload.accessToken);
      state.status = 'authenticated';
    },
    loggedOut(state) {
      state.accessToken = null;
      state.accessTokenExpiresAt = null;
      state.user = null;
      state.status = 'unauthenticated';
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(bootstrapAuth.fulfilled, (state, action) => {
        state.accessToken = action.payload.accessToken;
        state.accessTokenExpiresAt = action.payload.expiresAt;
        state.user = decodeAccessToken(action.payload.accessToken);
        state.status = 'authenticated';
      })
      .addCase(bootstrapAuth.rejected, (state) => {
        state.accessToken = null;
        state.accessTokenExpiresAt = null;
        state.user = null;
        state.status = 'unauthenticated';
      });
  },
});

export const { credentialsReceived, loggedOut } = authSlice.actions;
export default authSlice.reducer;

export const selectCurrentUser = (state: RootState) => state.auth.user;
export const selectAccessToken = (state: RootState) => state.auth.accessToken;
export const selectAuthStatus = (state: RootState) => state.auth.status;
