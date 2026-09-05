import { apiSlice } from './generated/apiSlice';

export const notificationsApi = apiSlice.enhanceEndpoints({
  addTagTypes: ['Notification'],
  endpoints: {
    getApiNotifications: {
      providesTags: ['Notification'],
    },
    postApiNotificationsByIdRead: {
      invalidatesTags: ['Notification'],
    },
  },
});

export const { useGetApiNotificationsQuery, usePostApiNotificationsByIdReadMutation } = notificationsApi;
