import { z } from 'zod';
import type { TFunction } from 'i18next';

// Mirrors CreateVehicleCommandValidator (Application/Vehicles/Commands/CreateVehicle).
export function buildVehicleSchema(t: TFunction) {
  return z.object({
    registrationNumber: z
      .string()
      .min(1, t('common:validation.required'))
      .max(30, t('common:validation.maxLength', { max: 30 })),
    vehicleType: z.number({ error: t('common:validation.required') }),
  });
}

export type VehicleFormValues = z.infer<ReturnType<typeof buildVehicleSchema>>;

// Mirrors AllocateVehicleCommandValidator.
export function buildAllocateVehicleSchema(t: TFunction) {
  return z.object({
    employeeId: z.string().min(1, t('common:validation.required')),
    assignedDate: z.string().min(1, t('common:validation.required')),
  });
}

export type AllocateVehicleFormValues = z.infer<ReturnType<typeof buildAllocateVehicleSchema>>;

// Mirrors ReturnVehicleCommandValidator.
export function buildReturnVehicleSchema(t: TFunction) {
  return z.object({
    returnedDate: z.string().min(1, t('common:validation.required')),
  });
}

export type ReturnVehicleFormValues = z.infer<ReturnType<typeof buildReturnVehicleSchema>>;

// Mirrors AddVehicleServiceRecordCommandValidator.
export function buildServiceRecordSchema(t: TFunction) {
  return z.object({
    serviceDate: z.string().min(1, t('common:validation.required')),
    odometer: z.number({ error: t('common:validation.required') }).min(0, t('validation.nonNegative')),
    description: z
      .string()
      .min(1, t('common:validation.required'))
      .max(500, t('common:validation.maxLength', { max: 500 })),
    cost: z.number({ error: t('common:validation.required') }).min(0, t('validation.nonNegative')),
  });
}

export type ServiceRecordFormValues = z.infer<ReturnType<typeof buildServiceRecordSchema>>;

// Mirrors AddVehicleOilChangeRecordCommandValidator.
export function buildOilChangeSchema(t: TFunction) {
  return z.object({
    changeDate: z.string().min(1, t('common:validation.required')),
    odometer: z.number({ error: t('common:validation.required') }).min(0, t('validation.nonNegative')),
    cost: z.number({ error: t('common:validation.required') }).min(0, t('validation.nonNegative')),
  });
}

export type OilChangeFormValues = z.infer<ReturnType<typeof buildOilChangeSchema>>;

// Mirrors AddVehicleTyreReplacementRecordCommandValidator.
export function buildTyreReplacementSchema(t: TFunction) {
  return z.object({
    replacementDate: z.string().min(1, t('common:validation.required')),
    odometer: z.number({ error: t('common:validation.required') }).min(0, t('validation.nonNegative')),
    numberOfTyres: z
      .number({ error: t('common:validation.required') })
      .gt(0, t('validation.tyresPositive'))
      .max(6, t('validation.tyresMax')),
    cost: z.number({ error: t('common:validation.required') }).min(0, t('validation.nonNegative')),
  });
}

export type TyreReplacementFormValues = z.infer<ReturnType<typeof buildTyreReplacementSchema>>;

// Mirrors AddVehicleAccidentRecordCommandValidator.
export function buildAccidentRecordSchema(t: TFunction) {
  return z.object({
    accidentDate: z.string().min(1, t('common:validation.required')),
    description: z
      .string()
      .min(1, t('common:validation.required'))
      .max(500, t('common:validation.maxLength', { max: 500 })),
    repairCost: z.number({ error: t('common:validation.required') }).min(0, t('validation.nonNegative')),
  });
}

export type AccidentRecordFormValues = z.infer<ReturnType<typeof buildAccidentRecordSchema>>;
