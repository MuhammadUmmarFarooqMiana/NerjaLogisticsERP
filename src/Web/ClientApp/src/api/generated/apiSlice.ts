import { baseApi as api } from "../baseApi";
const injectedRtkApi = api.injectEndpoints({
  endpoints: (build) => ({
    postApiAdvances: build.mutation<
      PostApiAdvancesApiResponse,
      PostApiAdvancesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Advances`,
        method: "POST",
        body: queryArg.createAdvanceCommand,
      }),
    }),
    getApiAdvances: build.query<
      GetApiAdvancesApiResponse,
      GetApiAdvancesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Advances`,
        params: {
          employeeId: queryArg.employeeId,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    getApiAdvancesMe: build.query<
      GetApiAdvancesMeApiResponse,
      GetApiAdvancesMeApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Advances/me`,
        params: {
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    postApiAuthRegister: build.mutation<
      PostApiAuthRegisterApiResponse,
      PostApiAuthRegisterApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Auth/register`,
        method: "POST",
        body: queryArg.registerEmployeeCommand,
      }),
    }),
    postApiAuthLogin: build.mutation<
      PostApiAuthLoginApiResponse,
      PostApiAuthLoginApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Auth/login`,
        method: "POST",
        body: queryArg.loginCommand,
      }),
    }),
    postApiAuthRefreshtoken: build.mutation<
      PostApiAuthRefreshtokenApiResponse,
      PostApiAuthRefreshtokenApiArg
    >({
      query: () => ({ url: `/api/Auth/refreshtoken`, method: "POST" }),
    }),
    postApiAuthChangePassword: build.mutation<
      PostApiAuthChangePasswordApiResponse,
      PostApiAuthChangePasswordApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Auth/change-password`,
        method: "POST",
        body: queryArg.changePasswordCommand,
      }),
    }),
    postApiAuthLogout: build.mutation<
      PostApiAuthLogoutApiResponse,
      PostApiAuthLogoutApiArg
    >({
      query: () => ({ url: `/api/Auth/logout`, method: "POST" }),
    }),
    postApiCompanyDocuments: build.mutation<
      PostApiCompanyDocumentsApiResponse,
      PostApiCompanyDocumentsApiArg
    >({
      query: (queryArg) => ({
        url: `/api/CompanyDocuments`,
        method: "POST",
        body: queryArg.body,
      }),
    }),
    getApiCompanyDocuments: build.query<
      GetApiCompanyDocumentsApiResponse,
      GetApiCompanyDocumentsApiArg
    >({
      query: (queryArg) => ({
        url: `/api/CompanyDocuments`,
        params: {
          folderId: queryArg.folderId,
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    getApiCompanyDocumentsById: build.query<
      GetApiCompanyDocumentsByIdApiResponse,
      GetApiCompanyDocumentsByIdApiArg
    >({
      query: (queryArg) => ({ url: `/api/CompanyDocuments/${queryArg.id}` }),
    }),
    deleteApiCompanyDocumentsById: build.mutation<
      DeleteApiCompanyDocumentsByIdApiResponse,
      DeleteApiCompanyDocumentsByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/CompanyDocuments/${queryArg.id}`,
        method: "DELETE",
      }),
    }),
    getApiCompanyDocumentsByIdDownload: build.query<
      GetApiCompanyDocumentsByIdDownloadApiResponse,
      GetApiCompanyDocumentsByIdDownloadApiArg
    >({
      query: (queryArg) => ({
        url: `/api/CompanyDocuments/${queryArg.id}/download`,
      }),
    }),
    getApiCompanyDocumentsFolders: build.query<
      GetApiCompanyDocumentsFoldersApiResponse,
      GetApiCompanyDocumentsFoldersApiArg
    >({
      query: (queryArg) => ({
        url: `/api/CompanyDocuments/folders`,
        params: {
          parentFolderId: queryArg.parentFolderId,
        },
      }),
    }),
    postApiCompanyDocumentsFolders: build.mutation<
      PostApiCompanyDocumentsFoldersApiResponse,
      PostApiCompanyDocumentsFoldersApiArg
    >({
      query: (queryArg) => ({
        url: `/api/CompanyDocuments/folders`,
        method: "POST",
        body: queryArg.createCompanyDocumentFolderCommand,
      }),
    }),
    deleteApiCompanyDocumentsFoldersById: build.mutation<
      DeleteApiCompanyDocumentsFoldersByIdApiResponse,
      DeleteApiCompanyDocumentsFoldersByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/CompanyDocuments/folders/${queryArg.id}`,
        method: "DELETE",
      }),
    }),
    postApiDailyOrdersUpsertOrderCount: build.mutation<
      PostApiDailyOrdersUpsertOrderCountApiResponse,
      PostApiDailyOrdersUpsertOrderCountApiArg
    >({
      query: (queryArg) => ({
        url: `/api/DailyOrders/UpsertOrderCount`,
        method: "POST",
        body: queryArg.upsertDailyOrderCommand,
      }),
    }),
    getApiDailyOrdersMe: build.query<
      GetApiDailyOrdersMeApiResponse,
      GetApiDailyOrdersMeApiArg
    >({
      query: () => ({ url: `/api/DailyOrders/me` }),
    }),
    postApiDailyOrdersClose: build.mutation<
      PostApiDailyOrdersCloseApiResponse,
      PostApiDailyOrdersCloseApiArg
    >({
      query: () => ({ url: `/api/DailyOrders/close`, method: "POST" }),
    }),
    getApiDailyOrders: build.query<
      GetApiDailyOrdersApiResponse,
      GetApiDailyOrdersApiArg
    >({
      query: (queryArg) => ({
        url: `/api/DailyOrders`,
        params: {
          date: queryArg.date,
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    getApiDailyOrdersPendingApprovalsCount: build.query<
      GetApiDailyOrdersPendingApprovalsCountApiResponse,
      GetApiDailyOrdersPendingApprovalsCountApiArg
    >({
      query: () => ({ url: `/api/DailyOrders/pending-approvals-count` }),
    }),
    getApiDailyOrdersHistory: build.query<
      GetApiDailyOrdersHistoryApiResponse,
      GetApiDailyOrdersHistoryApiArg
    >({
      query: (queryArg) => ({
        url: `/api/DailyOrders/history`,
        params: {
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    postApiDailyOrdersByIdApprove: build.mutation<
      PostApiDailyOrdersByIdApproveApiResponse,
      PostApiDailyOrdersByIdApproveApiArg
    >({
      query: (queryArg) => ({
        url: `/api/DailyOrders/${queryArg.id}/approve`,
        method: "POST",
      }),
    }),
    postApiDailyOrdersByIdReject: build.mutation<
      PostApiDailyOrdersByIdRejectApiResponse,
      PostApiDailyOrdersByIdRejectApiArg
    >({
      query: (queryArg) => ({
        url: `/api/DailyOrders/${queryArg.id}/reject`,
        method: "POST",
        body: queryArg.body,
      }),
    }),
    postApiDailyOrdersByIdCorrect: build.mutation<
      PostApiDailyOrdersByIdCorrectApiResponse,
      PostApiDailyOrdersByIdCorrectApiArg
    >({
      query: (queryArg) => ({
        url: `/api/DailyOrders/${queryArg.id}/correct`,
        method: "POST",
        body: queryArg.correctDailyOrderCommand,
      }),
    }),
    postApiEmployeeDocuments: build.mutation<
      PostApiEmployeeDocumentsApiResponse,
      PostApiEmployeeDocumentsApiArg
    >({
      query: (queryArg) => ({
        url: `/api/EmployeeDocuments`,
        method: "POST",
        body: queryArg.body,
      }),
    }),
    getApiEmployeeDocuments: build.query<
      GetApiEmployeeDocumentsApiResponse,
      GetApiEmployeeDocumentsApiArg
    >({
      query: () => ({ url: `/api/EmployeeDocuments` }),
    }),
    getApiEmployeeDocumentsEmployeeByEmployeeId: build.query<
      GetApiEmployeeDocumentsEmployeeByEmployeeIdApiResponse,
      GetApiEmployeeDocumentsEmployeeByEmployeeIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/EmployeeDocuments/employee/${queryArg.employeeId}`,
      }),
    }),
    getApiEmployeeDocumentsPlatformByPlatformId: build.query<
      GetApiEmployeeDocumentsPlatformByPlatformIdApiResponse,
      GetApiEmployeeDocumentsPlatformByPlatformIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/EmployeeDocuments/platform/${queryArg.platformId}`,
      }),
    }),
    getApiEmployeeDocumentsById: build.query<
      GetApiEmployeeDocumentsByIdApiResponse,
      GetApiEmployeeDocumentsByIdApiArg
    >({
      query: (queryArg) => ({ url: `/api/EmployeeDocuments/${queryArg.id}` }),
    }),
    getApiEmployeeDocumentsByIdDownload: build.query<
      GetApiEmployeeDocumentsByIdDownloadApiResponse,
      GetApiEmployeeDocumentsByIdDownloadApiArg
    >({
      query: (queryArg) => ({
        url: `/api/EmployeeDocuments/${queryArg.id}/download`,
      }),
    }),
    getApiEmployees: build.query<
      GetApiEmployeesApiResponse,
      GetApiEmployeesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Employees`,
        params: {
          status: queryArg.status,
          platformId: queryArg.platformId,
          searchTerm: queryArg.searchTerm,
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    postApiEmployees: build.mutation<
      PostApiEmployeesApiResponse,
      PostApiEmployeesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Employees`,
        method: "POST",
        body: queryArg.adminCreateEmployeeCommand,
      }),
    }),
    getApiEmployeesById: build.query<
      GetApiEmployeesByIdApiResponse,
      GetApiEmployeesByIdApiArg
    >({
      query: (queryArg) => ({ url: `/api/Employees/${queryArg.id}` }),
    }),
    putApiEmployeesById: build.mutation<
      PutApiEmployeesByIdApiResponse,
      PutApiEmployeesByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Employees/${queryArg.id}`,
        method: "PUT",
        body: queryArg.adminUpdateEmployeeCommand,
      }),
    }),
    deleteApiEmployeesById: build.mutation<
      DeleteApiEmployeesByIdApiResponse,
      DeleteApiEmployeesByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Employees/${queryArg.id}`,
        method: "DELETE",
      }),
    }),
    getApiEmployeesMe: build.query<
      GetApiEmployeesMeApiResponse,
      GetApiEmployeesMeApiArg
    >({
      query: () => ({ url: `/api/Employees/me` }),
    }),
    getApiEmployeesMeProfilePicture: build.query<
      GetApiEmployeesMeProfilePictureApiResponse,
      GetApiEmployeesMeProfilePictureApiArg
    >({
      query: () => ({ url: `/api/Employees/me/profile-picture` }),
    }),
    postApiEmployeesMeProfilePicture: build.mutation<
      PostApiEmployeesMeProfilePictureApiResponse,
      PostApiEmployeesMeProfilePictureApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Employees/me/profile-picture`,
        method: "POST",
        body: queryArg.body,
      }),
    }),
    deleteApiEmployeesMeProfilePicture: build.mutation<
      DeleteApiEmployeesMeProfilePictureApiResponse,
      DeleteApiEmployeesMeProfilePictureApiArg
    >({
      query: () => ({
        url: `/api/Employees/me/profile-picture`,
        method: "DELETE",
      }),
    }),
    getApiEmployeesSupervisors: build.query<
      GetApiEmployeesSupervisorsApiResponse,
      GetApiEmployeesSupervisorsApiArg
    >({
      query: () => ({ url: `/api/Employees/supervisors` }),
    }),
    postApiEmployeesByIdSubmitProfile: build.mutation<
      PostApiEmployeesByIdSubmitProfileApiResponse,
      PostApiEmployeesByIdSubmitProfileApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Employees/${queryArg.id}/submitProfile`,
        method: "POST",
        body: queryArg.submitProfileForReviewCommand,
      }),
    }),
    postApiEmployeesByIdApprove: build.mutation<
      PostApiEmployeesByIdApproveApiResponse,
      PostApiEmployeesByIdApproveApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Employees/${queryArg.id}/approve`,
        method: "POST",
        body: queryArg.approveEmployeeCommand,
      }),
    }),
    postApiEmployeesByIdReject: build.mutation<
      PostApiEmployeesByIdRejectApiResponse,
      PostApiEmployeesByIdRejectApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Employees/${queryArg.id}/reject`,
        method: "POST",
        body: queryArg.rejectEmployeeCommand,
      }),
    }),
    putApiEmployeesIdUpdateProfile: build.mutation<
      PutApiEmployeesIdUpdateProfileApiResponse,
      PutApiEmployeesIdUpdateProfileApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Employees/Id/UpdateProfile`,
        method: "PUT",
        body: queryArg.updateEmployeeProfileCommand,
      }),
    }),
    postApiEmployeesByIdSuspend: build.mutation<
      PostApiEmployeesByIdSuspendApiResponse,
      PostApiEmployeesByIdSuspendApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Employees/${queryArg.id}/suspend`,
        method: "POST",
      }),
    }),
    postApiEmployeesByIdReactivate: build.mutation<
      PostApiEmployeesByIdReactivateApiResponse,
      PostApiEmployeesByIdReactivateApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Employees/${queryArg.id}/reactivate`,
        method: "POST",
      }),
    }),
    postApiEmployeesByIdTerminate: build.mutation<
      PostApiEmployeesByIdTerminateApiResponse,
      PostApiEmployeesByIdTerminateApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Employees/${queryArg.id}/terminate`,
        method: "POST",
      }),
    }),
    postApiExpenses: build.mutation<
      PostApiExpensesApiResponse,
      PostApiExpensesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Expenses`,
        method: "POST",
        body: queryArg.createExpenseCommand,
      }),
    }),
    getApiExpenses: build.query<
      GetApiExpensesApiResponse,
      GetApiExpensesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Expenses`,
        params: {
          category: queryArg.category,
          platformId: queryArg.platformId,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    postApiFines: build.mutation<PostApiFinesApiResponse, PostApiFinesApiArg>({
      query: (queryArg) => ({
        url: `/api/Fines`,
        method: "POST",
        body: queryArg.createFineCommand,
      }),
    }),
    getApiFines: build.query<GetApiFinesApiResponse, GetApiFinesApiArg>({
      query: (queryArg) => ({
        url: `/api/Fines`,
        params: {
          employeeId: queryArg.employeeId,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    getApiFinesMe: build.query<GetApiFinesMeApiResponse, GetApiFinesMeApiArg>({
      query: (queryArg) => ({
        url: `/api/Fines/me`,
        params: {
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    getApiInventoryItems: build.query<
      GetApiInventoryItemsApiResponse,
      GetApiInventoryItemsApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Inventory/items`,
        params: {
          lowStockOnly: queryArg.lowStockOnly,
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    postApiInventoryItems: build.mutation<
      PostApiInventoryItemsApiResponse,
      PostApiInventoryItemsApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Inventory/items`,
        method: "POST",
        body: queryArg.createInventoryItemCommand,
      }),
    }),
    getApiInventoryItemsByItemIdHistory: build.query<
      GetApiInventoryItemsByItemIdHistoryApiResponse,
      GetApiInventoryItemsByItemIdHistoryApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Inventory/items/${queryArg.itemId}/history`,
        params: {
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    postApiInventoryStockIn: build.mutation<
      PostApiInventoryStockInApiResponse,
      PostApiInventoryStockInApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Inventory/stock-in`,
        method: "POST",
        body: queryArg.recordStockInCommand,
      }),
    }),
    postApiInventoryStockOut: build.mutation<
      PostApiInventoryStockOutApiResponse,
      PostApiInventoryStockOutApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Inventory/stock-out`,
        method: "POST",
        body: queryArg.recordStockOutCommand,
      }),
    }),
    getApiInventoryLedger: build.query<
      GetApiInventoryLedgerApiResponse,
      GetApiInventoryLedgerApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Inventory/ledger`,
        params: {
          itemId: queryArg.itemId,
          fromDate: queryArg.fromDate,
          toDate: queryArg.toDate,
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    getApiLeaveRequestsMe: build.query<
      GetApiLeaveRequestsMeApiResponse,
      GetApiLeaveRequestsMeApiArg
    >({
      query: (queryArg) => ({
        url: `/api/LeaveRequests/me`,
        params: {
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    getApiLeaveRequests: build.query<
      GetApiLeaveRequestsApiResponse,
      GetApiLeaveRequestsApiArg
    >({
      query: (queryArg) => ({
        url: `/api/LeaveRequests`,
        params: {
          status: queryArg.status,
          employeeId: queryArg.employeeId,
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    postApiLeaveRequests: build.mutation<
      PostApiLeaveRequestsApiResponse,
      PostApiLeaveRequestsApiArg
    >({
      query: (queryArg) => ({
        url: `/api/LeaveRequests`,
        method: "POST",
        body: queryArg.submitLeaveRequestCommand,
      }),
    }),
    postApiLeaveRequestsByIdApprove: build.mutation<
      PostApiLeaveRequestsByIdApproveApiResponse,
      PostApiLeaveRequestsByIdApproveApiArg
    >({
      query: (queryArg) => ({
        url: `/api/LeaveRequests/${queryArg.id}/approve`,
        method: "POST",
      }),
    }),
    postApiLeaveRequestsByIdReject: build.mutation<
      PostApiLeaveRequestsByIdRejectApiResponse,
      PostApiLeaveRequestsByIdRejectApiArg
    >({
      query: (queryArg) => ({
        url: `/api/LeaveRequests/${queryArg.id}/reject`,
        method: "POST",
        body: queryArg.body,
      }),
    }),
    getApiMechanics: build.query<
      GetApiMechanicsApiResponse,
      GetApiMechanicsApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Mechanics`,
        params: {
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    postApiMechanics: build.mutation<
      PostApiMechanicsApiResponse,
      PostApiMechanicsApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Mechanics`,
        method: "POST",
        body: queryArg.createMechanicCommand,
      }),
    }),
    getApiMechanicsById: build.query<
      GetApiMechanicsByIdApiResponse,
      GetApiMechanicsByIdApiArg
    >({
      query: (queryArg) => ({ url: `/api/Mechanics/${queryArg.id}` }),
    }),
    putApiMechanicsById: build.mutation<
      PutApiMechanicsByIdApiResponse,
      PutApiMechanicsByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Mechanics/${queryArg.id}`,
        method: "PUT",
        body: queryArg.updateMechanicCommand,
      }),
    }),
    deleteApiMechanicsById: build.mutation<
      DeleteApiMechanicsByIdApiResponse,
      DeleteApiMechanicsByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Mechanics/${queryArg.id}`,
        method: "DELETE",
      }),
    }),
    getApiMonthlySummaries: build.query<
      GetApiMonthlySummariesApiResponse,
      GetApiMonthlySummariesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/MonthlySummaries`,
        params: {
          employeeId: queryArg.employeeId,
          year: queryArg.year,
          month: queryArg.month,
          status: queryArg.status,
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    getApiMonthlySummariesById: build.query<
      GetApiMonthlySummariesByIdApiResponse,
      GetApiMonthlySummariesByIdApiArg
    >({
      query: (queryArg) => ({ url: `/api/MonthlySummaries/${queryArg.id}` }),
    }),
    getApiMonthlySummariesMe: build.query<
      GetApiMonthlySummariesMeApiResponse,
      GetApiMonthlySummariesMeApiArg
    >({
      query: (queryArg) => ({
        url: `/api/MonthlySummaries/me`,
        params: {
          year: queryArg.year,
          month: queryArg.month,
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    postApiMonthlySummariesGenerate: build.mutation<
      PostApiMonthlySummariesGenerateApiResponse,
      PostApiMonthlySummariesGenerateApiArg
    >({
      query: (queryArg) => ({
        url: `/api/MonthlySummaries/generate`,
        method: "POST",
        body: queryArg.generateMonthlySummaryCommand,
      }),
    }),
    postApiMonthlySummariesByIdVerify: build.mutation<
      PostApiMonthlySummariesByIdVerifyApiResponse,
      PostApiMonthlySummariesByIdVerifyApiArg
    >({
      query: (queryArg) => ({
        url: `/api/MonthlySummaries/${queryArg.id}/verify`,
        method: "POST",
      }),
    }),
    postApiMonthlySummariesByIdMarkAsPaid: build.mutation<
      PostApiMonthlySummariesByIdMarkAsPaidApiResponse,
      PostApiMonthlySummariesByIdMarkAsPaidApiArg
    >({
      query: (queryArg) => ({
        url: `/api/MonthlySummaries/${queryArg.id}/MarkAsPaid`,
        method: "POST",
        body: queryArg.body,
      }),
    }),
    getApiNotifications: build.query<
      GetApiNotificationsApiResponse,
      GetApiNotificationsApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Notifications`,
        params: {
          unreadOnly: queryArg.unreadOnly,
        },
      }),
    }),
    postApiNotificationsByIdRead: build.mutation<
      PostApiNotificationsByIdReadApiResponse,
      PostApiNotificationsByIdReadApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Notifications/${queryArg.id}/read`,
        method: "POST",
      }),
    }),
    getApiPlatforms: build.query<
      GetApiPlatformsApiResponse,
      GetApiPlatformsApiArg
    >({
      query: () => ({ url: `/api/Platforms` }),
    }),
    postApiPlatforms: build.mutation<
      PostApiPlatformsApiResponse,
      PostApiPlatformsApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Platforms`,
        method: "POST",
        body: queryArg.createPlatformCommand,
      }),
    }),
    getApiPlatformsById: build.query<
      GetApiPlatformsByIdApiResponse,
      GetApiPlatformsByIdApiArg
    >({
      query: (queryArg) => ({ url: `/api/Platforms/${queryArg.id}` }),
    }),
    putApiPlatformsById: build.mutation<
      PutApiPlatformsByIdApiResponse,
      PutApiPlatformsByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Platforms/${queryArg.id}`,
        method: "PUT",
        body: queryArg.updatePlatformCommand,
      }),
    }),
    deleteApiPlatformsById: build.mutation<
      DeleteApiPlatformsByIdApiResponse,
      DeleteApiPlatformsByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Platforms/${queryArg.id}`,
        method: "DELETE",
      }),
    }),
    getApiReportsOrders: build.query<
      GetApiReportsOrdersApiResponse,
      GetApiReportsOrdersApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/orders`,
        params: {
          periodType: queryArg.periodType,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          platformId: queryArg.platformId,
          employeeId: queryArg.employeeId,
        },
      }),
    }),
    getApiReportsOrdersExport: build.query<
      GetApiReportsOrdersExportApiResponse,
      GetApiReportsOrdersExportApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/orders/export`,
        params: {
          periodType: queryArg.periodType,
          format: queryArg.format,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          platformId: queryArg.platformId,
          employeeId: queryArg.employeeId,
        },
      }),
    }),
    getApiReportsFines: build.query<
      GetApiReportsFinesApiResponse,
      GetApiReportsFinesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/fines`,
        params: {
          periodType: queryArg.periodType,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          employeeId: queryArg.employeeId,
        },
      }),
    }),
    getApiReportsFinesExport: build.query<
      GetApiReportsFinesExportApiResponse,
      GetApiReportsFinesExportApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/fines/export`,
        params: {
          periodType: queryArg.periodType,
          format: queryArg.format,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          employeeId: queryArg.employeeId,
        },
      }),
    }),
    getApiReportsAdvances: build.query<
      GetApiReportsAdvancesApiResponse,
      GetApiReportsAdvancesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/advances`,
        params: {
          periodType: queryArg.periodType,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          employeeId: queryArg.employeeId,
        },
      }),
    }),
    getApiReportsAdvancesExport: build.query<
      GetApiReportsAdvancesExportApiResponse,
      GetApiReportsAdvancesExportApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/advances/export`,
        params: {
          periodType: queryArg.periodType,
          format: queryArg.format,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          employeeId: queryArg.employeeId,
        },
      }),
    }),
    getApiReportsExpenses: build.query<
      GetApiReportsExpensesApiResponse,
      GetApiReportsExpensesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/expenses`,
        params: {
          periodType: queryArg.periodType,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          category: queryArg.category,
          platformId: queryArg.platformId,
        },
      }),
    }),
    getApiReportsExpensesExport: build.query<
      GetApiReportsExpensesExportApiResponse,
      GetApiReportsExpensesExportApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/expenses/export`,
        params: {
          periodType: queryArg.periodType,
          format: queryArg.format,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          category: queryArg.category,
          platformId: queryArg.platformId,
        },
      }),
    }),
    getApiReportsLeaves: build.query<
      GetApiReportsLeavesApiResponse,
      GetApiReportsLeavesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/leaves`,
        params: {
          periodType: queryArg.periodType,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          status: queryArg.status,
          employeeId: queryArg.employeeId,
        },
      }),
    }),
    getApiReportsLeavesExport: build.query<
      GetApiReportsLeavesExportApiResponse,
      GetApiReportsLeavesExportApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/leaves/export`,
        params: {
          periodType: queryArg.periodType,
          format: queryArg.format,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          status: queryArg.status,
          employeeId: queryArg.employeeId,
        },
      }),
    }),
    getApiReportsVehicles: build.query<
      GetApiReportsVehiclesApiResponse,
      GetApiReportsVehiclesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/vehicles`,
        params: {
          periodType: queryArg.periodType,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          vehicleId: queryArg.vehicleId,
        },
      }),
    }),
    getApiReportsVehiclesExport: build.query<
      GetApiReportsVehiclesExportApiResponse,
      GetApiReportsVehiclesExportApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/vehicles/export`,
        params: {
          periodType: queryArg.periodType,
          format: queryArg.format,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          vehicleId: queryArg.vehicleId,
        },
      }),
    }),
    getApiReportsSuppliers: build.query<
      GetApiReportsSuppliersApiResponse,
      GetApiReportsSuppliersApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/suppliers`,
        params: {
          periodType: queryArg.periodType,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          supplierId: queryArg.supplierId,
        },
      }),
    }),
    getApiReportsSuppliersExport: build.query<
      GetApiReportsSuppliersExportApiResponse,
      GetApiReportsSuppliersExportApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/suppliers/export`,
        params: {
          periodType: queryArg.periodType,
          format: queryArg.format,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          supplierId: queryArg.supplierId,
        },
      }),
    }),
    getApiReportsInventoryLedger: build.query<
      GetApiReportsInventoryLedgerApiResponse,
      GetApiReportsInventoryLedgerApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/inventory-ledger`,
        params: {
          periodType: queryArg.periodType,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          itemId: queryArg.itemId,
        },
      }),
    }),
    getApiReportsInventoryLedgerExport: build.query<
      GetApiReportsInventoryLedgerExportApiResponse,
      GetApiReportsInventoryLedgerExportApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/inventory-ledger/export`,
        params: {
          periodType: queryArg.periodType,
          format: queryArg.format,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          itemId: queryArg.itemId,
        },
      }),
    }),
    getApiReportsSalaries: build.query<
      GetApiReportsSalariesApiResponse,
      GetApiReportsSalariesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/salaries`,
        params: {
          periodType: queryArg.periodType,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          status: queryArg.status,
          employeeId: queryArg.employeeId,
        },
      }),
    }),
    getApiReportsSalariesExport: build.query<
      GetApiReportsSalariesExportApiResponse,
      GetApiReportsSalariesExportApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/salaries/export`,
        params: {
          periodType: queryArg.periodType,
          format: queryArg.format,
          date: queryArg.date,
          year: queryArg.year,
          month: queryArg.month,
          startDate: queryArg.startDate,
          endDate: queryArg.endDate,
          status: queryArg.status,
          employeeId: queryArg.employeeId,
        },
      }),
    }),
    postApiReportsPlatformReconciliationGenerate: build.mutation<
      PostApiReportsPlatformReconciliationGenerateApiResponse,
      PostApiReportsPlatformReconciliationGenerateApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/platform-reconciliation/generate`,
        method: "POST",
        body: queryArg.body,
      }),
    }),
    postApiReportsPlatformReconciliationExport: build.mutation<
      PostApiReportsPlatformReconciliationExportApiResponse,
      PostApiReportsPlatformReconciliationExportApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Reports/platform-reconciliation/export`,
        method: "POST",
        body: queryArg.body,
      }),
    }),
    postApiSalaryFormulas: build.mutation<
      PostApiSalaryFormulasApiResponse,
      PostApiSalaryFormulasApiArg
    >({
      query: (queryArg) => ({
        url: `/api/SalaryFormulas`,
        method: "POST",
        body: queryArg.createSalaryFormulaCommand,
      }),
    }),
    getApiSalaryFormulas: build.query<
      GetApiSalaryFormulasApiResponse,
      GetApiSalaryFormulasApiArg
    >({
      query: (queryArg) => ({
        url: `/api/SalaryFormulas`,
        params: {
          platformId: queryArg.platformId,
          activeOnly: queryArg.activeOnly,
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    getApiSalaryFormulasPreview: build.query<
      GetApiSalaryFormulasPreviewApiResponse,
      GetApiSalaryFormulasPreviewApiArg
    >({
      query: (queryArg) => ({
        url: `/api/SalaryFormulas/preview`,
        params: {
          employeeId: queryArg.employeeId,
          year: queryArg.year,
          month: queryArg.month,
        },
      }),
    }),
    getApiSalaryFormulasById: build.query<
      GetApiSalaryFormulasByIdApiResponse,
      GetApiSalaryFormulasByIdApiArg
    >({
      query: (queryArg) => ({ url: `/api/SalaryFormulas/${queryArg.id}` }),
    }),
    putApiSalaryFormulasById: build.mutation<
      PutApiSalaryFormulasByIdApiResponse,
      PutApiSalaryFormulasByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/SalaryFormulas/${queryArg.id}`,
        method: "PUT",
        body: queryArg.updateSalaryFormulaCommand,
      }),
    }),
    postApiSalaryFormulasByIdDeactivate: build.mutation<
      PostApiSalaryFormulasByIdDeactivateApiResponse,
      PostApiSalaryFormulasByIdDeactivateApiArg
    >({
      query: (queryArg) => ({
        url: `/api/SalaryFormulas/${queryArg.id}/deactivate`,
        method: "POST",
      }),
    }),
    getApiSuppliers: build.query<
      GetApiSuppliersApiResponse,
      GetApiSuppliersApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Suppliers`,
        params: {
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    postApiSuppliers: build.mutation<
      PostApiSuppliersApiResponse,
      PostApiSuppliersApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Suppliers`,
        method: "POST",
        body: queryArg.createSupplierCommand,
      }),
    }),
    getApiSuppliersById: build.query<
      GetApiSuppliersByIdApiResponse,
      GetApiSuppliersByIdApiArg
    >({
      query: (queryArg) => ({ url: `/api/Suppliers/${queryArg.id}` }),
    }),
    putApiSuppliersById: build.mutation<
      PutApiSuppliersByIdApiResponse,
      PutApiSuppliersByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Suppliers/${queryArg.id}`,
        method: "PUT",
        body: queryArg.updateSupplierCommand,
      }),
    }),
    deleteApiSuppliersById: build.mutation<
      DeleteApiSuppliersByIdApiResponse,
      DeleteApiSuppliersByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Suppliers/${queryArg.id}`,
        method: "DELETE",
      }),
    }),
    getApiUsers: build.query<GetApiUsersApiResponse, GetApiUsersApiArg>({
      query: () => ({ url: `/api/Users` }),
    }),
    putApiUsersByIdRoles: build.mutation<
      PutApiUsersByIdRolesApiResponse,
      PutApiUsersByIdRolesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Users/${queryArg.id}/roles`,
        method: "PUT",
        body: queryArg.updateUserRolesCommand,
      }),
    }),
    getApiVehicles: build.query<
      GetApiVehiclesApiResponse,
      GetApiVehiclesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles`,
        params: {
          activeOnly: queryArg.activeOnly,
          pageNumber: queryArg.pageNumber,
          pageSize: queryArg.pageSize,
        },
      }),
    }),
    postApiVehicles: build.mutation<
      PostApiVehiclesApiResponse,
      PostApiVehiclesApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles`,
        method: "POST",
        body: queryArg.createVehicleCommand,
      }),
    }),
    getApiVehiclesById: build.query<
      GetApiVehiclesByIdApiResponse,
      GetApiVehiclesByIdApiArg
    >({
      query: (queryArg) => ({ url: `/api/Vehicles/${queryArg.id}` }),
    }),
    getApiVehiclesMe: build.query<
      GetApiVehiclesMeApiResponse,
      GetApiVehiclesMeApiArg
    >({
      query: () => ({ url: `/api/Vehicles/me` }),
    }),
    getApiVehiclesByIdHistory: build.query<
      GetApiVehiclesByIdHistoryApiResponse,
      GetApiVehiclesByIdHistoryApiArg
    >({
      query: (queryArg) => ({ url: `/api/Vehicles/${queryArg.id}/history` }),
    }),
    postApiVehiclesByIdActivate: build.mutation<
      PostApiVehiclesByIdActivateApiResponse,
      PostApiVehiclesByIdActivateApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/${queryArg.id}/activate`,
        method: "POST",
      }),
    }),
    postApiVehiclesByIdDeactivate: build.mutation<
      PostApiVehiclesByIdDeactivateApiResponse,
      PostApiVehiclesByIdDeactivateApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/${queryArg.id}/deactivate`,
        method: "POST",
      }),
    }),
    postApiVehiclesByIdService: build.mutation<
      PostApiVehiclesByIdServiceApiResponse,
      PostApiVehiclesByIdServiceApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/${queryArg.id}/service`,
        method: "POST",
        body: queryArg.addVehicleServiceRecordCommand,
      }),
    }),
    postApiVehiclesByIdOilChange: build.mutation<
      PostApiVehiclesByIdOilChangeApiResponse,
      PostApiVehiclesByIdOilChangeApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/${queryArg.id}/oil-change`,
        method: "POST",
        body: queryArg.addVehicleOilChangeRecordCommand,
      }),
    }),
    postApiVehiclesByIdTyreReplacement: build.mutation<
      PostApiVehiclesByIdTyreReplacementApiResponse,
      PostApiVehiclesByIdTyreReplacementApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/${queryArg.id}/tyre-replacement`,
        method: "POST",
        body: queryArg.addVehicleTyreReplacementRecordCommand,
      }),
    }),
    postApiVehiclesByIdAccident: build.mutation<
      PostApiVehiclesByIdAccidentApiResponse,
      PostApiVehiclesByIdAccidentApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/${queryArg.id}/accident`,
        method: "POST",
        body: queryArg.addVehicleAccidentRecordCommand,
      }),
    }),
    postApiVehiclesAllocate: build.mutation<
      PostApiVehiclesAllocateApiResponse,
      PostApiVehiclesAllocateApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/allocate`,
        method: "POST",
        body: queryArg.allocateVehicleCommand,
      }),
    }),
    postApiVehiclesReturn: build.mutation<
      PostApiVehiclesReturnApiResponse,
      PostApiVehiclesReturnApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/return`,
        method: "POST",
        body: queryArg.returnVehicleCommand,
      }),
    }),
    putApiVehiclesAllocationsById: build.mutation<
      PutApiVehiclesAllocationsByIdApiResponse,
      PutApiVehiclesAllocationsByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/allocations/${queryArg.id}`,
        method: "PUT",
        body: queryArg.updateVehicleAllocationCommand,
      }),
    }),
    deleteApiVehiclesAllocationsById: build.mutation<
      DeleteApiVehiclesAllocationsByIdApiResponse,
      DeleteApiVehiclesAllocationsByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/allocations/${queryArg.id}`,
        method: "DELETE",
      }),
    }),
    putApiVehiclesServiceById: build.mutation<
      PutApiVehiclesServiceByIdApiResponse,
      PutApiVehiclesServiceByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/service/${queryArg.id}`,
        method: "PUT",
        body: queryArg.updateVehicleServiceRecordCommand,
      }),
    }),
    deleteApiVehiclesServiceById: build.mutation<
      DeleteApiVehiclesServiceByIdApiResponse,
      DeleteApiVehiclesServiceByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/service/${queryArg.id}`,
        method: "DELETE",
      }),
    }),
    putApiVehiclesOilChangeById: build.mutation<
      PutApiVehiclesOilChangeByIdApiResponse,
      PutApiVehiclesOilChangeByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/oil-change/${queryArg.id}`,
        method: "PUT",
        body: queryArg.updateVehicleOilChangeRecordCommand,
      }),
    }),
    deleteApiVehiclesOilChangeById: build.mutation<
      DeleteApiVehiclesOilChangeByIdApiResponse,
      DeleteApiVehiclesOilChangeByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/oil-change/${queryArg.id}`,
        method: "DELETE",
      }),
    }),
    putApiVehiclesTyreReplacementById: build.mutation<
      PutApiVehiclesTyreReplacementByIdApiResponse,
      PutApiVehiclesTyreReplacementByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/tyre-replacement/${queryArg.id}`,
        method: "PUT",
        body: queryArg.updateVehicleTyreReplacementRecordCommand,
      }),
    }),
    deleteApiVehiclesTyreReplacementById: build.mutation<
      DeleteApiVehiclesTyreReplacementByIdApiResponse,
      DeleteApiVehiclesTyreReplacementByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/tyre-replacement/${queryArg.id}`,
        method: "DELETE",
      }),
    }),
    putApiVehiclesAccidentById: build.mutation<
      PutApiVehiclesAccidentByIdApiResponse,
      PutApiVehiclesAccidentByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/accident/${queryArg.id}`,
        method: "PUT",
        body: queryArg.updateVehicleAccidentRecordCommand,
      }),
    }),
    deleteApiVehiclesAccidentById: build.mutation<
      DeleteApiVehiclesAccidentByIdApiResponse,
      DeleteApiVehiclesAccidentByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Vehicles/accident/${queryArg.id}`,
        method: "DELETE",
      }),
    }),
  }),
  overrideExisting: false,
});
export { injectedRtkApi as apiSlice };
export type PostApiAdvancesApiResponse = unknown;
export type PostApiAdvancesApiArg = {
  createAdvanceCommand: CreateAdvanceCommand;
};
export type GetApiAdvancesApiResponse = /** status 200 OK */ AdvanceDto[];
export type GetApiAdvancesApiArg = {
  employeeId?: string;
  startDate?: string;
  endDate?: string;
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type GetApiAdvancesMeApiResponse = /** status 200 OK */ AdvanceDto[];
export type GetApiAdvancesMeApiArg = {
  startDate?: string;
  endDate?: string;
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type PostApiAuthRegisterApiResponse = /** status 200 OK */ string;
export type PostApiAuthRegisterApiArg = {
  registerEmployeeCommand: RegisterEmployeeCommand;
};
export type PostApiAuthLoginApiResponse =
  /** status 200 OK */ AuthTokenResponseDto;
export type PostApiAuthLoginApiArg = {
  loginCommand: LoginCommand;
};
export type PostApiAuthRefreshtokenApiResponse =
  /** status 200 OK */ AuthTokenResponseDto;
export type PostApiAuthRefreshtokenApiArg = void;
export type PostApiAuthChangePasswordApiResponse = unknown;
export type PostApiAuthChangePasswordApiArg = {
  changePasswordCommand: ChangePasswordCommand;
};
export type PostApiAuthLogoutApiResponse = unknown;
export type PostApiAuthLogoutApiArg = void;
export type PostApiCompanyDocumentsApiResponse = unknown;
export type PostApiCompanyDocumentsApiArg = {
  body: {
    title?: string;
  } & {
    category?: CompanyDocumentCategory;
  } & {
    folderId?: string;
  } & {
    file?: IFormFile;
  };
};
export type GetApiCompanyDocumentsApiResponse =
  /** status 200 OK */ CompanyDocumentDto[];
export type GetApiCompanyDocumentsApiArg = {
  folderId?: string;
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type GetApiCompanyDocumentsByIdApiResponse =
  /** status 200 OK */ CompanyDocumentDto;
export type GetApiCompanyDocumentsByIdApiArg = {
  id: string;
};
export type DeleteApiCompanyDocumentsByIdApiResponse = unknown;
export type DeleteApiCompanyDocumentsByIdApiArg = {
  id: string;
};
export type GetApiCompanyDocumentsByIdDownloadApiResponse = unknown;
export type GetApiCompanyDocumentsByIdDownloadApiArg = {
  id: string;
};
export type GetApiCompanyDocumentsFoldersApiResponse =
  /** status 200 OK */ CompanyDocumentFolderDto[];
export type GetApiCompanyDocumentsFoldersApiArg = {
  parentFolderId?: string;
};
export type PostApiCompanyDocumentsFoldersApiResponse = unknown;
export type PostApiCompanyDocumentsFoldersApiArg = {
  createCompanyDocumentFolderCommand: CreateCompanyDocumentFolderCommand;
};
export type DeleteApiCompanyDocumentsFoldersByIdApiResponse = unknown;
export type DeleteApiCompanyDocumentsFoldersByIdApiArg = {
  id: string;
};
export type PostApiDailyOrdersUpsertOrderCountApiResponse = unknown;
export type PostApiDailyOrdersUpsertOrderCountApiArg = {
  upsertDailyOrderCommand: UpsertDailyOrderCommand;
};
export type GetApiDailyOrdersMeApiResponse = /** status 200 OK */ DailyOrderDto;
export type GetApiDailyOrdersMeApiArg = void;
export type PostApiDailyOrdersCloseApiResponse = unknown;
export type PostApiDailyOrdersCloseApiArg = void;
export type GetApiDailyOrdersApiResponse =
  /** status 200 OK */ DailyOrderListItemDto[];
export type GetApiDailyOrdersApiArg = {
  date?: string;
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type GetApiDailyOrdersPendingApprovalsCountApiResponse =
  /** status 200 OK */ number | string;
export type GetApiDailyOrdersPendingApprovalsCountApiArg = void;
export type GetApiDailyOrdersHistoryApiResponse =
  /** status 200 OK */ DailyOrderListItemDto[];
export type GetApiDailyOrdersHistoryApiArg = {
  startDate?: string;
  endDate?: string;
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type PostApiDailyOrdersByIdApproveApiResponse = unknown;
export type PostApiDailyOrdersByIdApproveApiArg = {
  id: string;
};
export type PostApiDailyOrdersByIdRejectApiResponse = unknown;
export type PostApiDailyOrdersByIdRejectApiArg = {
  id: string;
  body: string;
};
export type PostApiDailyOrdersByIdCorrectApiResponse = unknown;
export type PostApiDailyOrdersByIdCorrectApiArg = {
  id: string;
  correctDailyOrderCommand: CorrectDailyOrderCommand;
};
export type PostApiEmployeeDocumentsApiResponse = unknown;
export type PostApiEmployeeDocumentsApiArg = {
  body: {
    employeeId?: string;
  } & {
    type?: EmployeeDocumentType;
  } & {
    file?: IFormFile;
  };
};
export type GetApiEmployeeDocumentsApiResponse =
  /** status 200 OK */ EmployeeDocumentDto[];
export type GetApiEmployeeDocumentsApiArg = void;
export type GetApiEmployeeDocumentsEmployeeByEmployeeIdApiResponse =
  /** status 200 OK */ EmployeeDocumentDto[];
export type GetApiEmployeeDocumentsEmployeeByEmployeeIdApiArg = {
  employeeId: string;
};
export type GetApiEmployeeDocumentsPlatformByPlatformIdApiResponse =
  /** status 200 OK */ EmployeeDocumentDto[];
export type GetApiEmployeeDocumentsPlatformByPlatformIdApiArg = {
  platformId: string;
};
export type GetApiEmployeeDocumentsByIdApiResponse =
  /** status 200 OK */ EmployeeDocumentDto;
export type GetApiEmployeeDocumentsByIdApiArg = {
  id: string;
};
export type GetApiEmployeeDocumentsByIdDownloadApiResponse = unknown;
export type GetApiEmployeeDocumentsByIdDownloadApiArg = {
  id: string;
};
export type GetApiEmployeesApiResponse =
  /** status 200 OK */ EmployeeListItemDto[];
export type GetApiEmployeesApiArg = {
  status?: string;
  platformId?: string;
  searchTerm?: string;
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type PostApiEmployeesApiResponse = unknown;
export type PostApiEmployeesApiArg = {
  adminCreateEmployeeCommand: AdminCreateEmployeeCommand;
};
export type GetApiEmployeesByIdApiResponse =
  /** status 200 OK */ EmployeeDetailDto;
export type GetApiEmployeesByIdApiArg = {
  id: string;
};
export type PutApiEmployeesByIdApiResponse = unknown;
export type PutApiEmployeesByIdApiArg = {
  id: string;
  adminUpdateEmployeeCommand: AdminUpdateEmployeeCommand;
};
export type DeleteApiEmployeesByIdApiResponse = unknown;
export type DeleteApiEmployeesByIdApiArg = {
  id: string;
};
export type GetApiEmployeesMeApiResponse =
  /** status 200 OK */ EmployeeDetailDto;
export type GetApiEmployeesMeApiArg = void;
export type GetApiEmployeesMeProfilePictureApiResponse = unknown;
export type GetApiEmployeesMeProfilePictureApiArg = void;
export type PostApiEmployeesMeProfilePictureApiResponse = unknown;
export type PostApiEmployeesMeProfilePictureApiArg = {
  body: {
    file?: IFormFile;
  };
};
export type DeleteApiEmployeesMeProfilePictureApiResponse = unknown;
export type DeleteApiEmployeesMeProfilePictureApiArg = void;
export type GetApiEmployeesSupervisorsApiResponse =
  /** status 200 OK */ SupervisorLookupDto[];
export type GetApiEmployeesSupervisorsApiArg = void;
export type PostApiEmployeesByIdSubmitProfileApiResponse = unknown;
export type PostApiEmployeesByIdSubmitProfileApiArg = {
  id: string;
  submitProfileForReviewCommand: SubmitProfileForReviewCommand;
};
export type PostApiEmployeesByIdApproveApiResponse = unknown;
export type PostApiEmployeesByIdApproveApiArg = {
  id: string;
  approveEmployeeCommand: ApproveEmployeeCommand;
};
export type PostApiEmployeesByIdRejectApiResponse = unknown;
export type PostApiEmployeesByIdRejectApiArg = {
  id: string;
  rejectEmployeeCommand: RejectEmployeeCommand;
};
export type PutApiEmployeesIdUpdateProfileApiResponse = unknown;
export type PutApiEmployeesIdUpdateProfileApiArg = {
  updateEmployeeProfileCommand: UpdateEmployeeProfileCommand;
};
export type PostApiEmployeesByIdSuspendApiResponse = unknown;
export type PostApiEmployeesByIdSuspendApiArg = {
  id: string;
};
export type PostApiEmployeesByIdReactivateApiResponse = unknown;
export type PostApiEmployeesByIdReactivateApiArg = {
  id: string;
};
export type PostApiEmployeesByIdTerminateApiResponse = unknown;
export type PostApiEmployeesByIdTerminateApiArg = {
  id: string;
};
export type PostApiExpensesApiResponse = unknown;
export type PostApiExpensesApiArg = {
  createExpenseCommand: CreateExpenseCommand;
};
export type GetApiExpensesApiResponse = /** status 200 OK */ ExpenseDto[];
export type GetApiExpensesApiArg = {
  category?: ExpenseCategory;
  platformId?: string;
  startDate?: string;
  endDate?: string;
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type PostApiFinesApiResponse = unknown;
export type PostApiFinesApiArg = {
  createFineCommand: CreateFineCommand;
};
export type GetApiFinesApiResponse = /** status 200 OK */ FineDto[];
export type GetApiFinesApiArg = {
  employeeId?: string;
  startDate?: string;
  endDate?: string;
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type GetApiFinesMeApiResponse = /** status 200 OK */ FineDto[];
export type GetApiFinesMeApiArg = {
  startDate?: string;
  endDate?: string;
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type GetApiInventoryItemsApiResponse =
  /** status 200 OK */ InventoryItemDto[];
export type GetApiInventoryItemsApiArg = {
  lowStockOnly?: boolean;
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type PostApiInventoryItemsApiResponse = unknown;
export type PostApiInventoryItemsApiArg = {
  createInventoryItemCommand: CreateInventoryItemCommand;
};
export type GetApiInventoryItemsByItemIdHistoryApiResponse =
  /** status 200 OK */ StockMovementDto[];
export type GetApiInventoryItemsByItemIdHistoryApiArg = {
  itemId: string;
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type PostApiInventoryStockInApiResponse = unknown;
export type PostApiInventoryStockInApiArg = {
  recordStockInCommand: RecordStockInCommand;
};
export type PostApiInventoryStockOutApiResponse = unknown;
export type PostApiInventoryStockOutApiArg = {
  recordStockOutCommand: RecordStockOutCommand;
};
export type GetApiInventoryLedgerApiResponse =
  /** status 200 OK */ StockLedgerEntryDto[];
export type GetApiInventoryLedgerApiArg = {
  itemId?: string;
  fromDate?: string;
  toDate?: string;
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type GetApiLeaveRequestsMeApiResponse =
  /** status 200 OK */ LeaveRequestDto[];
export type GetApiLeaveRequestsMeApiArg = {
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type GetApiLeaveRequestsApiResponse =
  /** status 200 OK */ LeaveRequestDto[];
export type GetApiLeaveRequestsApiArg = {
  status?: LeaveStatus;
  employeeId?: string;
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type PostApiLeaveRequestsApiResponse = unknown;
export type PostApiLeaveRequestsApiArg = {
  submitLeaveRequestCommand: SubmitLeaveRequestCommand;
};
export type PostApiLeaveRequestsByIdApproveApiResponse = unknown;
export type PostApiLeaveRequestsByIdApproveApiArg = {
  id: string;
};
export type PostApiLeaveRequestsByIdRejectApiResponse = unknown;
export type PostApiLeaveRequestsByIdRejectApiArg = {
  id: string;
  body: string;
};
export type GetApiMechanicsApiResponse = /** status 200 OK */ MechanicDto[];
export type GetApiMechanicsApiArg = {
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type PostApiMechanicsApiResponse = unknown;
export type PostApiMechanicsApiArg = {
  createMechanicCommand: CreateMechanicCommand;
};
export type GetApiMechanicsByIdApiResponse = /** status 200 OK */ MechanicDto;
export type GetApiMechanicsByIdApiArg = {
  id: string;
};
export type PutApiMechanicsByIdApiResponse = unknown;
export type PutApiMechanicsByIdApiArg = {
  id: string;
  updateMechanicCommand: UpdateMechanicCommand;
};
export type DeleteApiMechanicsByIdApiResponse = unknown;
export type DeleteApiMechanicsByIdApiArg = {
  id: string;
};
export type GetApiMonthlySummariesApiResponse =
  /** status 200 OK */ MonthlySummaryDto[];
export type GetApiMonthlySummariesApiArg = {
  employeeId?: string;
  year?: number | string;
  month?: number | string;
  status?: MonthlySummaryStatus;
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type GetApiMonthlySummariesByIdApiResponse =
  /** status 200 OK */ MonthlySummaryDto;
export type GetApiMonthlySummariesByIdApiArg = {
  id: string;
};
export type GetApiMonthlySummariesMeApiResponse =
  /** status 200 OK */ MonthlySummaryDto[];
export type GetApiMonthlySummariesMeApiArg = {
  year?: number | string;
  month?: number | string;
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type PostApiMonthlySummariesGenerateApiResponse = unknown;
export type PostApiMonthlySummariesGenerateApiArg = {
  generateMonthlySummaryCommand: GenerateMonthlySummaryCommand;
};
export type PostApiMonthlySummariesByIdVerifyApiResponse = unknown;
export type PostApiMonthlySummariesByIdVerifyApiArg = {
  id: string;
};
export type PostApiMonthlySummariesByIdMarkAsPaidApiResponse = unknown;
export type PostApiMonthlySummariesByIdMarkAsPaidApiArg = {
  id: string;
  body: null | string;
};
export type GetApiNotificationsApiResponse =
  /** status 200 OK */ NotificationDto[];
export type GetApiNotificationsApiArg = {
  unreadOnly?: boolean;
};
export type PostApiNotificationsByIdReadApiResponse = unknown;
export type PostApiNotificationsByIdReadApiArg = {
  id: string;
};
export type GetApiPlatformsApiResponse = /** status 200 OK */ PlatformDto[];
export type GetApiPlatformsApiArg = void;
export type PostApiPlatformsApiResponse = unknown;
export type PostApiPlatformsApiArg = {
  createPlatformCommand: CreatePlatformCommand;
};
export type GetApiPlatformsByIdApiResponse = /** status 200 OK */ PlatformDto;
export type GetApiPlatformsByIdApiArg = {
  id: string;
};
export type PutApiPlatformsByIdApiResponse = unknown;
export type PutApiPlatformsByIdApiArg = {
  id: string;
  updatePlatformCommand: UpdatePlatformCommand;
};
export type DeleteApiPlatformsByIdApiResponse = unknown;
export type DeleteApiPlatformsByIdApiArg = {
  id: string;
};
export type GetApiReportsOrdersApiResponse =
  /** status 200 OK */ OrdersReportDto;
export type GetApiReportsOrdersApiArg = {
  periodType?: ReportPeriodType;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  platformId?: string;
  employeeId?: string;
};
export type GetApiReportsOrdersExportApiResponse = unknown;
export type GetApiReportsOrdersExportApiArg = {
  periodType?: ReportPeriodType;
  format?: ReportFormat;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  platformId?: string;
  employeeId?: string;
};
export type GetApiReportsFinesApiResponse = /** status 200 OK */ FinesReportDto;
export type GetApiReportsFinesApiArg = {
  periodType?: ReportPeriodType;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  employeeId?: string;
};
export type GetApiReportsFinesExportApiResponse = unknown;
export type GetApiReportsFinesExportApiArg = {
  periodType?: ReportPeriodType;
  format?: ReportFormat;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  employeeId?: string;
};
export type GetApiReportsAdvancesApiResponse =
  /** status 200 OK */ AdvancesReportDto;
export type GetApiReportsAdvancesApiArg = {
  periodType?: ReportPeriodType;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  employeeId?: string;
};
export type GetApiReportsAdvancesExportApiResponse = unknown;
export type GetApiReportsAdvancesExportApiArg = {
  periodType?: ReportPeriodType;
  format?: ReportFormat;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  employeeId?: string;
};
export type GetApiReportsExpensesApiResponse =
  /** status 200 OK */ ExpensesReportDto;
export type GetApiReportsExpensesApiArg = {
  periodType?: ReportPeriodType;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  category?: ExpenseCategory;
  platformId?: string;
};
export type GetApiReportsExpensesExportApiResponse = unknown;
export type GetApiReportsExpensesExportApiArg = {
  periodType?: ReportPeriodType;
  format?: ReportFormat;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  category?: ExpenseCategory;
  platformId?: string;
};
export type GetApiReportsLeavesApiResponse =
  /** status 200 OK */ LeavesReportDto;
export type GetApiReportsLeavesApiArg = {
  periodType?: ReportPeriodType;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  status?: LeaveStatus;
  employeeId?: string;
};
export type GetApiReportsLeavesExportApiResponse = unknown;
export type GetApiReportsLeavesExportApiArg = {
  periodType?: ReportPeriodType;
  format?: ReportFormat;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  status?: LeaveStatus;
  employeeId?: string;
};
export type GetApiReportsVehiclesApiResponse =
  /** status 200 OK */ VehiclesReportDto;
export type GetApiReportsVehiclesApiArg = {
  periodType?: ReportPeriodType;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  vehicleId?: string;
};
export type GetApiReportsVehiclesExportApiResponse = unknown;
export type GetApiReportsVehiclesExportApiArg = {
  periodType?: ReportPeriodType;
  format?: ReportFormat;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  vehicleId?: string;
};
export type GetApiReportsSuppliersApiResponse =
  /** status 200 OK */ SuppliersReportDto;
export type GetApiReportsSuppliersApiArg = {
  periodType?: ReportPeriodType;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  supplierId?: string;
};
export type GetApiReportsSuppliersExportApiResponse = unknown;
export type GetApiReportsSuppliersExportApiArg = {
  periodType?: ReportPeriodType;
  format?: ReportFormat;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  supplierId?: string;
};
export type GetApiReportsInventoryLedgerApiResponse =
  /** status 200 OK */ InventoryLedgerReportDto;
export type GetApiReportsInventoryLedgerApiArg = {
  periodType?: ReportPeriodType;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  itemId?: string;
};
export type GetApiReportsInventoryLedgerExportApiResponse = unknown;
export type GetApiReportsInventoryLedgerExportApiArg = {
  periodType?: ReportPeriodType;
  format?: ReportFormat;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  itemId?: string;
};
export type GetApiReportsSalariesApiResponse =
  /** status 200 OK */ SalariesReportDto;
export type GetApiReportsSalariesApiArg = {
  periodType?: ReportPeriodType;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  status?: MonthlySummaryStatus;
  employeeId?: string;
};
export type GetApiReportsSalariesExportApiResponse = unknown;
export type GetApiReportsSalariesExportApiArg = {
  periodType?: ReportPeriodType;
  format?: ReportFormat;
  date?: string;
  year?: number | string;
  month?: number | string;
  startDate?: string;
  endDate?: string;
  status?: MonthlySummaryStatus;
  employeeId?: string;
};
export type PostApiReportsPlatformReconciliationGenerateApiResponse =
  /** status 200 OK */ PlatformReconciliationReportDto;
export type PostApiReportsPlatformReconciliationGenerateApiArg = {
  body: {
    year?: number | string;
  } & {
    month?: number | string;
  } & {
    platformId?: string;
  } & {
    file?: IFormFile;
  };
};
export type PostApiReportsPlatformReconciliationExportApiResponse = unknown;
export type PostApiReportsPlatformReconciliationExportApiArg = {
  body: {
    year?: number | string;
  } & {
    month?: number | string;
  } & {
    platformId?: string;
  } & {
    format?: ReportFormat;
  } & {
    file?: IFormFile;
  };
};
export type PostApiSalaryFormulasApiResponse = unknown;
export type PostApiSalaryFormulasApiArg = {
  createSalaryFormulaCommand: CreateSalaryFormulaCommand;
};
export type GetApiSalaryFormulasApiResponse =
  /** status 200 OK */ SalaryFormulaDto[];
export type GetApiSalaryFormulasApiArg = {
  platformId?: string;
  activeOnly?: boolean;
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type GetApiSalaryFormulasPreviewApiResponse =
  /** status 200 OK */ SalaryPreviewDto;
export type GetApiSalaryFormulasPreviewApiArg = {
  employeeId?: string;
  year?: number | string;
  month?: number | string;
};
export type GetApiSalaryFormulasByIdApiResponse =
  /** status 200 OK */ SalaryFormulaDto;
export type GetApiSalaryFormulasByIdApiArg = {
  id: string;
};
export type PutApiSalaryFormulasByIdApiResponse = unknown;
export type PutApiSalaryFormulasByIdApiArg = {
  id: string;
  updateSalaryFormulaCommand: UpdateSalaryFormulaCommand;
};
export type PostApiSalaryFormulasByIdDeactivateApiResponse = unknown;
export type PostApiSalaryFormulasByIdDeactivateApiArg = {
  id: string;
};
export type GetApiSuppliersApiResponse = /** status 200 OK */ SupplierDto[];
export type GetApiSuppliersApiArg = {
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type PostApiSuppliersApiResponse = unknown;
export type PostApiSuppliersApiArg = {
  createSupplierCommand: CreateSupplierCommand;
};
export type GetApiSuppliersByIdApiResponse = /** status 200 OK */ SupplierDto;
export type GetApiSuppliersByIdApiArg = {
  id: string;
};
export type PutApiSuppliersByIdApiResponse = unknown;
export type PutApiSuppliersByIdApiArg = {
  id: string;
  updateSupplierCommand: UpdateSupplierCommand;
};
export type DeleteApiSuppliersByIdApiResponse = unknown;
export type DeleteApiSuppliersByIdApiArg = {
  id: string;
};
export type GetApiUsersApiResponse = /** status 200 OK */ UserListItemDto[];
export type GetApiUsersApiArg = void;
export type PutApiUsersByIdRolesApiResponse = unknown;
export type PutApiUsersByIdRolesApiArg = {
  id: string;
  updateUserRolesCommand: UpdateUserRolesCommand;
};
export type GetApiVehiclesApiResponse = /** status 200 OK */ VehicleDto[];
export type GetApiVehiclesApiArg = {
  activeOnly?: boolean;
  pageNumber?: number | string;
  pageSize?: number | string;
};
export type PostApiVehiclesApiResponse = unknown;
export type PostApiVehiclesApiArg = {
  createVehicleCommand: CreateVehicleCommand;
};
export type GetApiVehiclesByIdApiResponse = /** status 200 OK */ VehicleDto;
export type GetApiVehiclesByIdApiArg = {
  id: string;
};
export type GetApiVehiclesMeApiResponse = /** status 200 OK */ VehicleDto;
export type GetApiVehiclesMeApiArg = void;
export type GetApiVehiclesByIdHistoryApiResponse =
  /** status 200 OK */ VehicleHistoryDto;
export type GetApiVehiclesByIdHistoryApiArg = {
  id: string;
};
export type PostApiVehiclesByIdActivateApiResponse = unknown;
export type PostApiVehiclesByIdActivateApiArg = {
  id: string;
};
export type PostApiVehiclesByIdDeactivateApiResponse = unknown;
export type PostApiVehiclesByIdDeactivateApiArg = {
  id: string;
};
export type PostApiVehiclesByIdServiceApiResponse = unknown;
export type PostApiVehiclesByIdServiceApiArg = {
  id: string;
  addVehicleServiceRecordCommand: AddVehicleServiceRecordCommand;
};
export type PostApiVehiclesByIdOilChangeApiResponse = unknown;
export type PostApiVehiclesByIdOilChangeApiArg = {
  id: string;
  addVehicleOilChangeRecordCommand: AddVehicleOilChangeRecordCommand;
};
export type PostApiVehiclesByIdTyreReplacementApiResponse = unknown;
export type PostApiVehiclesByIdTyreReplacementApiArg = {
  id: string;
  addVehicleTyreReplacementRecordCommand: AddVehicleTyreReplacementRecordCommand;
};
export type PostApiVehiclesByIdAccidentApiResponse = unknown;
export type PostApiVehiclesByIdAccidentApiArg = {
  id: string;
  addVehicleAccidentRecordCommand: AddVehicleAccidentRecordCommand;
};
export type PostApiVehiclesAllocateApiResponse = unknown;
export type PostApiVehiclesAllocateApiArg = {
  allocateVehicleCommand: AllocateVehicleCommand;
};
export type PostApiVehiclesReturnApiResponse = unknown;
export type PostApiVehiclesReturnApiArg = {
  returnVehicleCommand: ReturnVehicleCommand;
};
export type PutApiVehiclesAllocationsByIdApiResponse = unknown;
export type PutApiVehiclesAllocationsByIdApiArg = {
  id: string;
  updateVehicleAllocationCommand: UpdateVehicleAllocationCommand;
};
export type DeleteApiVehiclesAllocationsByIdApiResponse = unknown;
export type DeleteApiVehiclesAllocationsByIdApiArg = {
  id: string;
};
export type PutApiVehiclesServiceByIdApiResponse = unknown;
export type PutApiVehiclesServiceByIdApiArg = {
  id: string;
  updateVehicleServiceRecordCommand: UpdateVehicleServiceRecordCommand;
};
export type DeleteApiVehiclesServiceByIdApiResponse = unknown;
export type DeleteApiVehiclesServiceByIdApiArg = {
  id: string;
};
export type PutApiVehiclesOilChangeByIdApiResponse = unknown;
export type PutApiVehiclesOilChangeByIdApiArg = {
  id: string;
  updateVehicleOilChangeRecordCommand: UpdateVehicleOilChangeRecordCommand;
};
export type DeleteApiVehiclesOilChangeByIdApiResponse = unknown;
export type DeleteApiVehiclesOilChangeByIdApiArg = {
  id: string;
};
export type PutApiVehiclesTyreReplacementByIdApiResponse = unknown;
export type PutApiVehiclesTyreReplacementByIdApiArg = {
  id: string;
  updateVehicleTyreReplacementRecordCommand: UpdateVehicleTyreReplacementRecordCommand;
};
export type DeleteApiVehiclesTyreReplacementByIdApiResponse = unknown;
export type DeleteApiVehiclesTyreReplacementByIdApiArg = {
  id: string;
};
export type PutApiVehiclesAccidentByIdApiResponse = unknown;
export type PutApiVehiclesAccidentByIdApiArg = {
  id: string;
  updateVehicleAccidentRecordCommand: UpdateVehicleAccidentRecordCommand;
};
export type DeleteApiVehiclesAccidentByIdApiResponse = unknown;
export type DeleteApiVehiclesAccidentByIdApiArg = {
  id: string;
};
export type CreateAdvanceCommand = {
  employeeId?: string;
  amount?: number | string;
  advanceDate?: string;
  remarks?: null | string;
};
export type AdvanceDto = {
  id?: string;
  employeeId?: string;
  employeeName?: string;
  amount?: number | string;
  advanceDate?: string;
  remarks?: null | string;
};
export type RegisterEmployeeCommand = {
  fullName?: string;
  email?: string;
  password?: string;
  phoneNumber?: string;
  hasWhatsApp?: boolean;
};
export type AuthTokenResponseDto = {
  accessToken: string;
  expiresAt: string;
};
export type LoginCommand = {
  email?: string;
  password?: string;
};
export type ChangePasswordCommand = {
  currentPassword?: string;
  newPassword?: string;
  confirmNewPassword?: string;
};
export type CompanyDocumentCategory = number;
export type IFormFile = Blob;
export type CompanyDocumentDto = {
  id?: string;
  title?: string;
  category?: string;
  originalFileName?: string;
  folderId?: null | string;
  uploadedByName?: null | string;
  created?: string;
};
export type CompanyDocumentFolderDto = {
  id?: string;
  name?: string;
  parentFolderId?: null | string;
  createdByName?: null | string;
  created?: string;
};
export type CreateCompanyDocumentFolderCommand = {
  name?: string;
  parentFolderId?: null | string;
};
export type UpsertDailyOrderCommand = {
  completedOrders?: number | string;
};
export type DailyOrderDto = {
  id?: string;
  orderDate?: string;
  completedOrders?: number | string;
  status?: string;
  closedAt?: null | string;
  reviewNote?: null | string;
};
export type DailyOrderListItemDto = {
  id?: null | string;
  employeeId?: string;
  employeeName?: string;
  orderDate?: string;
  completedOrders?: number | string;
  status?: string;
  closedAt?: null | string;
  reviewNote?: null | string;
};
export type CorrectDailyOrderCommand = {
  dailyOrderId?: string;
  completedOrders?: number | string;
  reason?: string;
};
export type EmployeeDocumentType = number;
export type EmployeeDocumentDto = {
  id?: string;
  employeeId?: string;
  employeeName?: string;
  type?: string;
  originalFileName?: string;
  contentType?: string;
  created?: string;
};
export type EmployeeListItemDto = {
  id?: string;
  fullName?: string;
  accountStatus?: string;
  iqamaNumber?: null | string;
  platformName?: null | string;
  joiningDate?: null | string;
  roles?: string[];
};
export type AdminCreateEmployeeCommand = {
  fullName?: string;
  email?: string;
  password?: string;
  phoneNumber?: string;
  hasWhatsApp?: boolean;
  roles?: string[];
  iqamaNumber?: string;
  platformId?: null | string;
  platformIdNumber?: null | string;
  joiningDate?: null | string;
  idExpiryDate?: null | string;
  iqamaExpiryDate?: null | string;
  drivingLicenseExpiryDate?: null | string;
  insuranceExpiryDate?: null | string;
};
export type EmployeeDetailDto = {
  id?: string;
  fullName?: string;
  accountStatus?: string;
  performanceStatus?: string;
  iqamaNumber?: null | string;
  platformIdNumber?: null | string;
  platformId?: null | string;
  platformName?: null | string;
  vehicleId?: null | string;
  vehicleRegistrationNumber?: null | string;
  supervisorId?: null | string;
  supervisorName?: null | string;
  joiningDate?: null | string;
  idExpiryDate?: null | string;
  iqamaExpiryDate?: null | string;
  drivingLicenseExpiryDate?: null | string;
  insuranceExpiryDate?: null | string;
  profileSubmittedAt?: null | string;
  rejectionReason?: string;
  email?: null | string;
  phoneNumber?: null | string;
  hasWhatsApp?: boolean;
  emailConfirmed?: boolean;
};
export type AdminUpdateEmployeeCommand = {
  id?: string;
  fullName?: string;
  iqamaNumber?: null | string;
  platformId?: null | string;
  idExpiryDate?: null | string;
  iqamaExpiryDate?: null | string;
  drivingLicenseExpiryDate?: null | string;
  insuranceExpiryDate?: null | string;
};
export type SupervisorLookupDto = {
  id?: string;
  fullName?: string;
};
export type SubmitProfileForReviewCommand = {
  userId?: string;
  iqamaNumber?: string;
  platformIdNumber?: string;
  idExpiryDate?: null | string;
  iqamaExpiryDate?: null | string;
  drivingLicenseExpiryDate?: null | string;
  insuranceExpiryDate?: null | string;
};
export type ApproveEmployeeCommand = {
  employeeId?: string;
  joiningDate?: null | string;
  platformId?: null | string;
  vehicleId?: null | string;
  supervisorId?: null | string;
};
export type RejectEmployeeCommand = {
  employeeId?: string;
  reason?: string;
};
export type UpdateEmployeeProfileCommand = {
  iqamaNumber?: string;
  platformId?: string;
  platformIdNumber?: string;
  idExpiryDate?: null | string;
  iqamaExpiryDate?: null | string;
  drivingLicenseExpiryDate?: null | string;
  insuranceExpiryDate?: null | string;
};
export type ExpenseCategory = number;
export type CreateExpenseCommand = {
  category?: ExpenseCategory;
  amount?: number | string;
  expenseDate?: string;
  description?: null | string;
  platformId?: null | string;
};
export type ExpenseDto = {
  id?: string;
  category?: string;
  amount?: number | string;
  expenseDate?: string;
  description?: null | string;
  platformId?: null | string;
  platformName?: null | string;
};
export type CreateFineCommand = {
  employeeId?: string;
  amount?: number | string;
  reason?: string;
  fineDate?: string;
};
export type FineDto = {
  id?: string;
  employeeId?: string;
  employeeName?: string;
  amount?: number | string;
  reason?: string;
  fineDate?: string;
};
export type InventoryItemDto = {
  id: string;
  itemName: string;
  unit: string;
  reorderLevel: number | string;
  currentStock: number | string;
  isLowStock: boolean;
};
export type CreateInventoryItemCommand = {
  itemName?: string;
  unit?: string;
  reorderLevel?: number | string;
};
export type StockMovementDto = {
  id: string;
  type: string;
  quantity: number | string;
  date: string;
  detail: null | string;
};
export type RecordStockInCommand = {
  itemId?: string;
  quantity?: number | string;
  stockDate?: string;
  supplierId?: string;
};
export type RecordStockOutCommand = {
  itemId?: string;
  employeeId?: string;
  mechanicId?: string;
  quantity?: number | string;
  stockDate?: string;
};
export type StockLedgerEntryDto = {
  id?: string;
  itemId?: string;
  itemName?: string;
  type?: string;
  quantity?: number | string;
  date?: string;
  detail?: null | string;
};
export type LeaveRequestDto = {
  id?: string;
  employeeId?: string;
  employeeName?: string;
  startDate?: string;
  endDate?: string;
  reason?: null | string;
  status?: string;
  reviewedByName?: null | string;
  reviewedAt?: null | string;
  rejectionReason?: null | string;
};
export type LeaveStatus = number;
export type SubmitLeaveRequestCommand = {
  startDate?: string;
  endDate?: string;
  reason?: null | string;
};
export type MechanicDto = {
  id: string;
  name: string;
  phone: null | string;
  email: null | string;
  specialty: null | string;
  address: null | string;
};
export type CreateMechanicCommand = {
  name?: string;
  phone?: null | string;
  email?: null | string;
  specialty?: null | string;
  address?: null | string;
};
export type UpdateMechanicCommand = {
  id?: string;
  name?: string;
  phone?: null | string;
  email?: null | string;
  specialty?: null | string;
  address?: null | string;
};
export type MonthlySummaryDto = {
  id?: string;
  employeeId?: string;
  employeeName?: string;
  year?: number | string;
  month?: number | string;
  totalCompletedOrders?: number | string;
  totalSalary?: number | string;
  totalAdvances?: number | string;
  totalFines?: number | string;
  netSalaryPayable?: number | string;
  status?: string;
};
export type MonthlySummaryStatus = number;
export type GenerateMonthlySummaryCommand = {
  employeeId?: string;
  year?: number | string;
  month?: number | string;
};
export type NotificationDto = {
  id: string;
  type: string;
  title: string;
  message: string;
  isRead: boolean;
  created: string;
};
export type PlatformDto = {
  id?: string;
  name?: string;
};
export type CreatePlatformCommand = {
  name?: string;
};
export type UpdatePlatformCommand = {
  id?: string;
  name?: string;
};
export type OrdersReportPlatformBreakdownDto = {
  platformName?: string;
  completedOrders?: number | string;
};
export type OrdersReportRowDto = {
  employeeId?: string;
  employeeName?: string;
  platformName?: null | string;
  orderDate?: string;
  completedOrders?: number | string;
  status?: string;
};
export type OrdersReportDto = {
  periodType?: string;
  periodLabel?: string;
  periodStart?: string;
  periodEnd?: string;
  totalCompletedOrders?: number | string;
  totalSessions?: number | string;
  uniqueRiders?: number | string;
  byPlatform?: OrdersReportPlatformBreakdownDto[];
  rows?: OrdersReportRowDto[];
};
export type ReportPeriodType = number;
export type ReportFormat = number;
export type FinesReportRowDto = {
  employeeId?: string;
  employeeName?: string;
  amount?: number | string;
  reason?: string;
  fineDate?: string;
};
export type FinesReportDto = {
  periodType?: string;
  periodLabel?: string;
  periodStart?: string;
  periodEnd?: string;
  totalAmount?: number | string;
  totalCount?: number | string;
  rows?: FinesReportRowDto[];
};
export type AdvancesReportRowDto = {
  employeeId?: string;
  employeeName?: string;
  amount?: number | string;
  remarks?: null | string;
  advanceDate?: string;
};
export type AdvancesReportDto = {
  periodType?: string;
  periodLabel?: string;
  periodStart?: string;
  periodEnd?: string;
  totalAmount?: number | string;
  totalCount?: number | string;
  rows?: AdvancesReportRowDto[];
};
export type ExpensesReportCategoryBreakdownDto = {
  category?: string;
  amount?: number | string;
};
export type ExpensesReportRowDto = {
  category?: string;
  amount?: number | string;
  expenseDate?: string;
  description?: null | string;
  platformName?: null | string;
};
export type ExpensesReportDto = {
  periodType?: string;
  periodLabel?: string;
  periodStart?: string;
  periodEnd?: string;
  totalAmount?: number | string;
  totalCount?: number | string;
  byCategory?: ExpensesReportCategoryBreakdownDto[];
  rows?: ExpensesReportRowDto[];
};
export type LeavesReportRowDto = {
  employeeId?: string;
  employeeName?: string;
  startDate?: string;
  endDate?: string;
  days?: number | string;
  reason?: null | string;
  status?: string;
  reviewedByName?: null | string;
};
export type LeavesReportDto = {
  periodType?: string;
  periodLabel?: string;
  periodStart?: string;
  periodEnd?: string;
  totalRequests?: number | string;
  totalDays?: number | string;
  approvedCount?: number | string;
  rejectedCount?: number | string;
  pendingCount?: number | string;
  rows?: LeavesReportRowDto[];
};
export type VehiclesReportRowDto = {
  vehicleRegistration?: string;
  recordType?: string;
  date?: string;
  description?: null | string;
  cost?: null | number | string;
};
export type VehiclesReportDto = {
  periodType?: string;
  periodLabel?: string;
  periodStart?: string;
  periodEnd?: string;
  totalVehicles?: number | string;
  activeVehicles?: number | string;
  totalMaintenanceCost?: number | string;
  rows?: VehiclesReportRowDto[];
};
export type SuppliersReportRowDto = {
  supplierName?: string;
  itemName?: string;
  quantity?: number | string;
  stockDate?: string;
};
export type SuppliersReportDto = {
  periodType?: string;
  periodLabel?: string;
  periodStart?: string;
  periodEnd?: string;
  totalSuppliers?: number | string;
  totalTransactions?: number | string;
  totalQuantityReceived?: number | string;
  rows?: SuppliersReportRowDto[];
};
export type InventoryLedgerReportRowDto = {
  date?: string;
  itemName?: string;
  movementType?: string;
  quantity?: number | string;
  reference?: string;
  runningBalance?: number | string;
};
export type InventoryLedgerReportDto = {
  periodType?: string;
  periodLabel?: string;
  periodStart?: string;
  periodEnd?: string;
  totalIn?: number | string;
  totalOut?: number | string;
  netChange?: number | string;
  rows?: InventoryLedgerReportRowDto[];
};
export type SalariesReportRowDto = {
  employeeId?: string;
  employeeName?: string;
  platformIdNumber?: null | string;
  year?: number | string;
  month?: number | string;
  totalCompletedOrders?: number | string;
  totalSalary?: number | string;
  totalAdvances?: number | string;
  totalFines?: number | string;
  netSalaryPayable?: number | string;
  status?: string;
};
export type SalariesReportDto = {
  periodType?: string;
  periodLabel?: string;
  periodStart?: string;
  periodEnd?: string;
  totalSalary?: number | string;
  totalAdvances?: number | string;
  totalFines?: number | string;
  totalNetPayable?: number | string;
  totalCount?: number | string;
  rows?: SalariesReportRowDto[];
};
export type PlatformReconciliationRowDto = {
  employeeId?: string;
  employeeName?: string;
  platformIdNumber?: string;
  nerjaCompletedOrders?: number | string;
  platformCompletedOrders?: number | string;
  ordersDifference?: number | string;
  stackingDeduction?: number | string;
  declinedPenaltiesDayLogic?: number | string;
  latePenalty?: number | string;
  noShowPenalty?: number | string;
  noShowPenaltySpecialCities?: number | string;
  dailyAcceptanceRatePenalty?: number | string;
  missedDaysPenalty?: number | string;
  totalPenalties?: number | string;
  originalNetPayable?: number | string;
  adjustedNetPayable?: number | string;
  monthlySummaryStatus?: string;
};
export type PlatformReconciliationUnmatchedPlatformRowDto = {
  platformIdNumber?: string;
  platformCompletedOrders?: number | string;
  totalPenalties?: number | string;
};
export type PlatformReconciliationMissingFromSheetDto = {
  employeeId?: string;
  employeeName?: string;
  platformIdNumber?: null | string;
  nerjaCompletedOrders?: number | string;
  netPayable?: number | string;
};
export type PlatformReconciliationReportDto = {
  year?: number | string;
  month?: number | string;
  platformName?: string;
  periodLabel?: string;
  matchedRows?: PlatformReconciliationRowDto[];
  unmatchedPlatformRows?: PlatformReconciliationUnmatchedPlatformRowDto[];
  missingFromSheetRows?: PlatformReconciliationMissingFromSheetDto[];
  totalMatched?: number | string;
  totalUnmatchedInPlatform?: number | string;
  totalMissingFromSheet?: number | string;
  totalOrdersMismatchCount?: number | string;
  totalPenalties?: number | string;
  totalOriginalNetPayable?: number | string;
  totalAdjustedNetPayable?: number | string;
};
export type SalaryFormulaType = number;
export type SalaryTierRateType = number;
export type SalaryTierInput = {
  minOrders: number | string;
  maxOrders: null | number | string;
  rateType: SalaryTierRateType;
  rate: number | string;
};
export type CreateSalaryFormulaCommand = {
  platformId?: null | string;
  formulaType?: SalaryFormulaType;
  fixedMonthlyAmount?: null | number | string;
  effectiveFrom?: string;
  tiers?: SalaryTierInput[];
};
export type SalaryFormulaTierDto = {
  minOrders?: number | string;
  maxOrders?: null | number | string;
  rateType?: string;
  rate?: number | string;
};
export type SalaryFormulaDto = {
  id?: string;
  platformId?: null | string;
  platformName?: null | string;
  formulaType?: string;
  fixedMonthlyAmount?: null | number | string;
  effectiveFrom?: string;
  effectiveTo?: null | string;
  tiers?: SalaryFormulaTierDto[];
};
export type SalaryPreviewDto = {
  employeeId: string;
  year: number | string;
  month: number | string;
  totalOrders: number | string;
  calculatedSalary: number | string;
};
export type UpdateSalaryFormulaCommand = {
  id?: string;
  fixedMonthlyAmount?: null | number | string;
  tiers?: SalaryTierInput[];
};
export type SupplierDto = {
  id: string;
  name: string;
  email: null | string;
  phone: null | string;
  address: null | string;
};
export type CreateSupplierCommand = {
  name?: string;
  email?: null | string;
  phone?: null | string;
  address?: null | string;
};
export type UpdateSupplierCommand = {
  id?: string;
  name?: string;
  email?: null | string;
  phone?: null | string;
  address?: null | string;
};
export type UserListItemDto = {
  id?: string;
  email?: null | string;
  fullName?: null | string;
  roles?: string[];
};
export type UpdateUserRolesCommand = {
  userId?: string;
  roles?: string[];
};
export type VehicleDto = {
  id?: string;
  registrationNumber?: string;
  vehicleType?: string;
  isActive?: boolean;
  assignedEmployeeName?: null | string;
};
export type VehicleType = number;
export type CreateVehicleCommand = {
  registrationNumber?: string;
  vehicleType?: VehicleType;
};
export type AllocationRecordDto = {
  id: string;
  employeeId: string;
  employeeName: string;
  assignedDate: string;
  returnedDate: null | string;
};
export type ServiceRecordDto = {
  id: string;
  serviceDate: string;
  odometer: number | string;
  description: string;
  cost: number | string;
};
export type OilChangeRecordDto = {
  id: string;
  changeDate: string;
  odometer: number | string;
  cost: number | string;
};
export type TyreReplacementRecordDto = {
  id: string;
  replacementDate: string;
  odometer: number | string;
  numberOfTyres: number | string;
  cost: number | string;
};
export type AccidentRecordDto = {
  id: string;
  accidentDate: string;
  description: string;
  repairCost: number | string;
};
export type VehicleHistoryDto = {
  allocations?: AllocationRecordDto[];
  serviceHistory?: ServiceRecordDto[];
  oilChanges?: OilChangeRecordDto[];
  tyreReplacements?: TyreReplacementRecordDto[];
  accidentHistory?: AccidentRecordDto[];
};
export type AddVehicleServiceRecordCommand = {
  vehicleId?: string;
  serviceDate?: string;
  odometer?: number | string;
  description?: string;
  cost?: number | string;
};
export type AddVehicleOilChangeRecordCommand = {
  vehicleId?: string;
  changeDate?: string;
  odometer?: number | string;
  cost?: number | string;
};
export type AddVehicleTyreReplacementRecordCommand = {
  vehicleId?: string;
  replacementDate?: string;
  odometer?: number | string;
  numberOfTyres?: number | string;
  cost?: number | string;
};
export type AddVehicleAccidentRecordCommand = {
  vehicleId?: string;
  accidentDate?: string;
  description?: string;
  repairCost?: number | string;
};
export type AllocateVehicleCommand = {
  vehicleId?: string;
  employeeId?: string;
  assignedDate?: string;
};
export type ReturnVehicleCommand = {
  allocationId?: string;
  returnedDate?: string;
};
export type UpdateVehicleAllocationCommand = {
  id?: string;
  employeeId?: string;
  assignedDate?: string;
};
export type UpdateVehicleServiceRecordCommand = {
  id?: string;
  serviceDate?: string;
  odometer?: number | string;
  description?: string;
  cost?: number | string;
};
export type UpdateVehicleOilChangeRecordCommand = {
  id?: string;
  changeDate?: string;
  odometer?: number | string;
  cost?: number | string;
};
export type UpdateVehicleTyreReplacementRecordCommand = {
  id?: string;
  replacementDate?: string;
  odometer?: number | string;
  numberOfTyres?: number | string;
  cost?: number | string;
};
export type UpdateVehicleAccidentRecordCommand = {
  id?: string;
  accidentDate?: string;
  description?: string;
  repairCost?: number | string;
};
export const {
  usePostApiAdvancesMutation,
  useGetApiAdvancesQuery,
  useGetApiAdvancesMeQuery,
  usePostApiAuthRegisterMutation,
  usePostApiAuthLoginMutation,
  usePostApiAuthRefreshtokenMutation,
  usePostApiAuthChangePasswordMutation,
  usePostApiAuthLogoutMutation,
  usePostApiCompanyDocumentsMutation,
  useGetApiCompanyDocumentsQuery,
  useGetApiCompanyDocumentsByIdQuery,
  useDeleteApiCompanyDocumentsByIdMutation,
  useGetApiCompanyDocumentsByIdDownloadQuery,
  useGetApiCompanyDocumentsFoldersQuery,
  usePostApiCompanyDocumentsFoldersMutation,
  useDeleteApiCompanyDocumentsFoldersByIdMutation,
  usePostApiDailyOrdersUpsertOrderCountMutation,
  useGetApiDailyOrdersMeQuery,
  usePostApiDailyOrdersCloseMutation,
  useGetApiDailyOrdersQuery,
  useGetApiDailyOrdersPendingApprovalsCountQuery,
  useGetApiDailyOrdersHistoryQuery,
  usePostApiDailyOrdersByIdApproveMutation,
  usePostApiDailyOrdersByIdRejectMutation,
  usePostApiDailyOrdersByIdCorrectMutation,
  usePostApiEmployeeDocumentsMutation,
  useGetApiEmployeeDocumentsQuery,
  useGetApiEmployeeDocumentsEmployeeByEmployeeIdQuery,
  useGetApiEmployeeDocumentsPlatformByPlatformIdQuery,
  useGetApiEmployeeDocumentsByIdQuery,
  useGetApiEmployeeDocumentsByIdDownloadQuery,
  useGetApiEmployeesQuery,
  usePostApiEmployeesMutation,
  useGetApiEmployeesByIdQuery,
  usePutApiEmployeesByIdMutation,
  useDeleteApiEmployeesByIdMutation,
  useGetApiEmployeesMeQuery,
  useGetApiEmployeesMeProfilePictureQuery,
  usePostApiEmployeesMeProfilePictureMutation,
  useDeleteApiEmployeesMeProfilePictureMutation,
  useGetApiEmployeesSupervisorsQuery,
  usePostApiEmployeesByIdSubmitProfileMutation,
  usePostApiEmployeesByIdApproveMutation,
  usePostApiEmployeesByIdRejectMutation,
  usePutApiEmployeesIdUpdateProfileMutation,
  usePostApiEmployeesByIdSuspendMutation,
  usePostApiEmployeesByIdReactivateMutation,
  usePostApiEmployeesByIdTerminateMutation,
  usePostApiExpensesMutation,
  useGetApiExpensesQuery,
  usePostApiFinesMutation,
  useGetApiFinesQuery,
  useGetApiFinesMeQuery,
  useGetApiInventoryItemsQuery,
  usePostApiInventoryItemsMutation,
  useGetApiInventoryItemsByItemIdHistoryQuery,
  usePostApiInventoryStockInMutation,
  usePostApiInventoryStockOutMutation,
  useGetApiInventoryLedgerQuery,
  useGetApiLeaveRequestsMeQuery,
  useGetApiLeaveRequestsQuery,
  usePostApiLeaveRequestsMutation,
  usePostApiLeaveRequestsByIdApproveMutation,
  usePostApiLeaveRequestsByIdRejectMutation,
  useGetApiMechanicsQuery,
  usePostApiMechanicsMutation,
  useGetApiMechanicsByIdQuery,
  usePutApiMechanicsByIdMutation,
  useDeleteApiMechanicsByIdMutation,
  useGetApiMonthlySummariesQuery,
  useGetApiMonthlySummariesByIdQuery,
  useGetApiMonthlySummariesMeQuery,
  usePostApiMonthlySummariesGenerateMutation,
  usePostApiMonthlySummariesByIdVerifyMutation,
  usePostApiMonthlySummariesByIdMarkAsPaidMutation,
  useGetApiNotificationsQuery,
  usePostApiNotificationsByIdReadMutation,
  useGetApiPlatformsQuery,
  usePostApiPlatformsMutation,
  useGetApiPlatformsByIdQuery,
  usePutApiPlatformsByIdMutation,
  useDeleteApiPlatformsByIdMutation,
  useGetApiReportsOrdersQuery,
  useGetApiReportsOrdersExportQuery,
  useGetApiReportsFinesQuery,
  useGetApiReportsFinesExportQuery,
  useGetApiReportsAdvancesQuery,
  useGetApiReportsAdvancesExportQuery,
  useGetApiReportsExpensesQuery,
  useGetApiReportsExpensesExportQuery,
  useGetApiReportsLeavesQuery,
  useGetApiReportsLeavesExportQuery,
  useGetApiReportsVehiclesQuery,
  useGetApiReportsVehiclesExportQuery,
  useGetApiReportsSuppliersQuery,
  useGetApiReportsSuppliersExportQuery,
  useGetApiReportsInventoryLedgerQuery,
  useGetApiReportsInventoryLedgerExportQuery,
  useGetApiReportsSalariesQuery,
  useGetApiReportsSalariesExportQuery,
  usePostApiReportsPlatformReconciliationGenerateMutation,
  usePostApiReportsPlatformReconciliationExportMutation,
  usePostApiSalaryFormulasMutation,
  useGetApiSalaryFormulasQuery,
  useGetApiSalaryFormulasPreviewQuery,
  useGetApiSalaryFormulasByIdQuery,
  usePutApiSalaryFormulasByIdMutation,
  usePostApiSalaryFormulasByIdDeactivateMutation,
  useGetApiSuppliersQuery,
  usePostApiSuppliersMutation,
  useGetApiSuppliersByIdQuery,
  usePutApiSuppliersByIdMutation,
  useDeleteApiSuppliersByIdMutation,
  useGetApiUsersQuery,
  usePutApiUsersByIdRolesMutation,
  useGetApiVehiclesQuery,
  usePostApiVehiclesMutation,
  useGetApiVehiclesByIdQuery,
  useGetApiVehiclesMeQuery,
  useGetApiVehiclesByIdHistoryQuery,
  usePostApiVehiclesByIdActivateMutation,
  usePostApiVehiclesByIdDeactivateMutation,
  usePostApiVehiclesByIdServiceMutation,
  usePostApiVehiclesByIdOilChangeMutation,
  usePostApiVehiclesByIdTyreReplacementMutation,
  usePostApiVehiclesByIdAccidentMutation,
  usePostApiVehiclesAllocateMutation,
  usePostApiVehiclesReturnMutation,
  usePutApiVehiclesAllocationsByIdMutation,
  useDeleteApiVehiclesAllocationsByIdMutation,
  usePutApiVehiclesServiceByIdMutation,
  useDeleteApiVehiclesServiceByIdMutation,
  usePutApiVehiclesOilChangeByIdMutation,
  useDeleteApiVehiclesOilChangeByIdMutation,
  usePutApiVehiclesTyreReplacementByIdMutation,
  useDeleteApiVehiclesTyreReplacementByIdMutation,
  usePutApiVehiclesAccidentByIdMutation,
  useDeleteApiVehiclesAccidentByIdMutation,
} = injectedRtkApi;
