import { apiSlice } from './generated/apiSlice';
import type { EmployeeDocumentType } from './generated/apiSlice';

interface UploadEmployeeDocumentArg {
  file: File;
  type: EmployeeDocumentType;
  employeeId?: string;
}

export const employeeDocumentsApi = apiSlice
  .enhanceEndpoints({
    addTagTypes: ['EmployeeDocument'],
    endpoints: {
      getApiEmployeeDocuments: {
        providesTags: ['EmployeeDocument'],
      },
      getApiEmployeeDocumentsEmployeeByEmployeeId: {
        providesTags: ['EmployeeDocument'],
      },
      postApiEmployeeDocuments: {
        invalidatesTags: ['EmployeeDocument'],
      },
    },
  })
  .injectEndpoints({
    endpoints: (build) => ({
      uploadEmployeeDocument: build.mutation<unknown, UploadEmployeeDocumentArg>({
        query: ({ file, type, employeeId }) => {
          const formData = new FormData();
          formData.append('file', file);
          formData.append('type', String(type));
          if (employeeId) formData.append('employeeId', employeeId);
          return {
            url: '/api/EmployeeDocuments',
            method: 'POST',
            body: formData,
          };
        },
        invalidatesTags: ['EmployeeDocument'],
      }),
    }),
  });

export const {
  useGetApiEmployeeDocumentsQuery,
  useGetApiEmployeeDocumentsEmployeeByEmployeeIdQuery,
  useUploadEmployeeDocumentMutation,
} = employeeDocumentsApi;

export function employeeDocumentDownloadUrl(id: string) {
  return `/api/EmployeeDocuments/${id}/download`;
}
