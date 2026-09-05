import { apiSlice } from './generated/apiSlice';
import { attachPaginationMeta } from '../lib/pagination';

export const expensesApi = apiSlice.enhanceEndpoints({
  addTagTypes: ['Expense'],
  endpoints: {
    getApiExpenses: {
      providesTags: ['Expense'],
      transformResponse: attachPaginationMeta,
    },
    postApiExpenses: {
      invalidatesTags: ['Expense'],
    },
  },
});

export const { useGetApiExpensesQuery, usePostApiExpensesMutation } = expensesApi;
