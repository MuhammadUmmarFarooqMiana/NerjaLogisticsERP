import { apiSlice } from './generated/apiSlice';
import { attachPaginationMeta } from '../lib/pagination';

export const salaryFormulasApi = apiSlice.enhanceEndpoints({
  addTagTypes: ['SalaryFormula'],
  endpoints: {
    getApiSalaryFormulas: {
      providesTags: ['SalaryFormula'],
      transformResponse: attachPaginationMeta,
    },
    getApiSalaryFormulasById: {
      providesTags: ['SalaryFormula'],
    },
    postApiSalaryFormulas: {
      invalidatesTags: ['SalaryFormula'],
    },
    putApiSalaryFormulasById: {
      invalidatesTags: ['SalaryFormula'],
    },
    postApiSalaryFormulasByIdDeactivate: {
      invalidatesTags: ['SalaryFormula'],
    },
  },
});

export const {
  useGetApiSalaryFormulasQuery,
  useGetApiSalaryFormulasByIdQuery,
  usePostApiSalaryFormulasMutation,
  usePutApiSalaryFormulasByIdMutation,
  usePostApiSalaryFormulasByIdDeactivateMutation,
} = salaryFormulasApi;
