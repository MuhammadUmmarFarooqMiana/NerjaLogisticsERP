import { apiSlice } from './generated/apiSlice';
import type { CompanyDocumentCategory } from './generated/apiSlice';
import { attachPaginationMeta } from '../lib/pagination';

interface UploadCompanyDocumentArg {
  file: File;
  title: string;
  category: CompanyDocumentCategory;
  /** Undefined/omitted uploads to the root, outside any folder. */
  folderId?: string;
}

export const companyDocumentsApi = apiSlice
  .enhanceEndpoints({
    addTagTypes: ['CompanyDocument', 'CompanyDocumentFolder'],
    endpoints: {
      getApiCompanyDocuments: {
        providesTags: ['CompanyDocument'],
        transformResponse: attachPaginationMeta,
      },
      postApiCompanyDocuments: {
        invalidatesTags: ['CompanyDocument'],
      },
      getApiCompanyDocumentsFolders: {
        providesTags: ['CompanyDocumentFolder'],
      },
      postApiCompanyDocumentsFolders: {
        invalidatesTags: ['CompanyDocumentFolder'],
      },
      deleteApiCompanyDocumentsFoldersById: {
        invalidatesTags: ['CompanyDocumentFolder'],
      },
    },
  })
  .injectEndpoints({
    endpoints: (build) => ({
      uploadCompanyDocument: build.mutation<unknown, UploadCompanyDocumentArg>({
        query: ({ file, title, category, folderId }) => {
          const formData = new FormData();
          formData.append('file', file);
          formData.append('title', title);
          formData.append('category', String(category));
          if (folderId) formData.append('folderId', folderId);
          return {
            url: '/api/CompanyDocuments',
            method: 'POST',
            body: formData,
          };
        },
        invalidatesTags: ['CompanyDocument'],
      }),
      // Hand-written like uploadCompanyDocument above — the backend route exists
      // (DeleteCompanyDocumentCommand / CompanyDocumentsController.Delete) but the
      // generated client hasn't been regenerated from a fresh OpenAPI spec yet.
      deleteCompanyDocument: build.mutation<unknown, { id: string }>({
        query: ({ id }) => ({
          url: `/api/CompanyDocuments/${id}`,
          method: 'DELETE',
        }),
        invalidatesTags: ['CompanyDocument'],
      }),
    }),
  });

export const {
  useGetApiCompanyDocumentsQuery,
  useUploadCompanyDocumentMutation,
  useDeleteCompanyDocumentMutation,
  useGetApiCompanyDocumentsFoldersQuery,
  usePostApiCompanyDocumentsFoldersMutation,
  useDeleteApiCompanyDocumentsFoldersByIdMutation,
} = companyDocumentsApi;

export function companyDocumentDownloadUrl(id: string) {
  return `/api/CompanyDocuments/${id}/download`;
}
