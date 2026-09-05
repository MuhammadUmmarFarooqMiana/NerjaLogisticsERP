import { apiSlice } from './generated/apiSlice';
import { attachPaginationMeta } from '../lib/pagination';

export const vehiclesApi = apiSlice.enhanceEndpoints({
  addTagTypes: ['Vehicle', 'VehicleHistory'],
  endpoints: {
    getApiVehicles: {
      providesTags: ['Vehicle'],
      transformResponse: attachPaginationMeta,
    },
    getApiVehiclesById: {
      providesTags: ['Vehicle'],
    },
    getApiVehiclesMe: {
      providesTags: ['Vehicle'],
    },
    getApiVehiclesByIdHistory: {
      providesTags: ['VehicleHistory'],
    },
    postApiVehicles: {
      invalidatesTags: ['Vehicle'],
    },
    postApiVehiclesByIdActivate: {
      invalidatesTags: ['Vehicle'],
    },
    postApiVehiclesByIdDeactivate: {
      invalidatesTags: ['Vehicle'],
    },
    postApiVehiclesByIdService: {
      invalidatesTags: ['VehicleHistory'],
    },
    postApiVehiclesByIdOilChange: {
      invalidatesTags: ['VehicleHistory'],
    },
    postApiVehiclesByIdTyreReplacement: {
      invalidatesTags: ['VehicleHistory'],
    },
    postApiVehiclesByIdAccident: {
      invalidatesTags: ['VehicleHistory'],
    },
    // Allocate/Return/Update/Delete-allocation all change who a vehicle is
    // currently assigned to, which shows up both in the Vehicles list
    // (assignedEmployeeName) and the Allocations history section.
    postApiVehiclesAllocate: {
      invalidatesTags: ['Vehicle', 'VehicleHistory'],
    },
    postApiVehiclesReturn: {
      invalidatesTags: ['Vehicle', 'VehicleHistory'],
    },
    putApiVehiclesAllocationsById: {
      invalidatesTags: ['Vehicle', 'VehicleHistory'],
    },
    deleteApiVehiclesAllocationsById: {
      invalidatesTags: ['Vehicle', 'VehicleHistory'],
    },
    putApiVehiclesServiceById: {
      invalidatesTags: ['VehicleHistory'],
    },
    deleteApiVehiclesServiceById: {
      invalidatesTags: ['VehicleHistory'],
    },
    putApiVehiclesOilChangeById: {
      invalidatesTags: ['VehicleHistory'],
    },
    deleteApiVehiclesOilChangeById: {
      invalidatesTags: ['VehicleHistory'],
    },
    putApiVehiclesTyreReplacementById: {
      invalidatesTags: ['VehicleHistory'],
    },
    deleteApiVehiclesTyreReplacementById: {
      invalidatesTags: ['VehicleHistory'],
    },
    putApiVehiclesAccidentById: {
      invalidatesTags: ['VehicleHistory'],
    },
    deleteApiVehiclesAccidentById: {
      invalidatesTags: ['VehicleHistory'],
    },
  },
});

export const {
  useGetApiVehiclesQuery,
  useGetApiVehiclesByIdQuery,
  useGetApiVehiclesMeQuery,
  useGetApiVehiclesByIdHistoryQuery,
  usePostApiVehiclesMutation,
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
} = vehiclesApi;
