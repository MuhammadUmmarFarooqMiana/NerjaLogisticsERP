import { apiSlice } from './generated/apiSlice';
import { attachPaginationMeta } from '../lib/pagination';

export const inventoryApi = apiSlice.enhanceEndpoints({
  addTagTypes: ['InventoryItem', 'StockLedger'],
  endpoints: {
    getApiInventoryItems: {
      providesTags: ['InventoryItem'],
      transformResponse: attachPaginationMeta,
    },
    getApiInventoryItemsByItemIdHistory: {
      providesTags: ['StockLedger'],
      transformResponse: attachPaginationMeta,
    },
    getApiInventoryLedger: {
      providesTags: ['StockLedger'],
      transformResponse: attachPaginationMeta,
    },
    postApiInventoryItems: {
      invalidatesTags: ['InventoryItem'],
    },
    postApiInventoryStockIn: {
      invalidatesTags: ['InventoryItem', 'StockLedger'],
    },
    postApiInventoryStockOut: {
      invalidatesTags: ['InventoryItem', 'StockLedger'],
    },
  },
});

export const {
  useGetApiInventoryItemsQuery,
  useGetApiInventoryItemsByItemIdHistoryQuery,
  useGetApiInventoryLedgerQuery,
  usePostApiInventoryItemsMutation,
  usePostApiInventoryStockInMutation,
  usePostApiInventoryStockOutMutation,
} = inventoryApi;
