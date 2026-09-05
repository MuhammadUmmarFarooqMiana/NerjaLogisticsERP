// Mirrors Domain/Enums/VehicleType.cs exactly — the API binds/returns this
// enum by its numeric ordinal (no JsonStringEnumConverter is configured), so
// order here must match the C# declaration order.
export const VEHICLE_TYPES = [
  { value: 0, key: 'Motorcycle' },
  { value: 1, key: 'Car' },
  { value: 2, key: 'Pickup' },
  { value: 3, key: 'Van' },
  { value: 4, key: 'MiniTruck' },
  { value: 5, key: 'Truck' },
  { value: 6, key: 'Trailer' },
  { value: 7, key: 'RefrigeratedTruck' },
  { value: 8, key: 'Forklift' },
  { value: 9, key: 'Other' },
] as const;
