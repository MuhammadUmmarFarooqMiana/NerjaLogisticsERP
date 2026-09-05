import { createSlice } from '@reduxjs/toolkit';
import type { RootState } from '../../app/store';

// Every mounted UserAvatar/useMyProfilePicture instance fetches its own blob
// independently (it can't be cached as JSON through RTK Query), so there's no
// single cache entry to invalidate after an upload/remove. This shared version
// counter is the signal every instance re-fetches on — bumping it anywhere
// (e.g. after uploading on MyProfilePage) refreshes every avatar on screen,
// including the one in TopBar.
const profilePictureSlice = createSlice({
  name: 'profilePicture',
  initialState: { version: 0 },
  reducers: {
    pictureChanged(state) {
      state.version += 1;
    },
  },
});

export const { pictureChanged } = profilePictureSlice.actions;
export default profilePictureSlice.reducer;

export const selectProfilePictureVersion = (state: RootState) => state.profilePicture.version;
