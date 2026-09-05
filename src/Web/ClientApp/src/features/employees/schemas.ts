import { z } from 'zod';
import type { TFunction } from 'i18next';

// Mirrors SubmitProfileForReviewCommandValidator
// (Application/Employees/Commands/SubmitProfileForReview). Date fields stay
// plain optional strings here — FormDatePicker's onChange yields "" when
// cleared, which the caller converts to undefined before hitting the API
// (the backend's DateOnly? can't parse an empty string).
export function buildProfileSubmissionSchema(t: TFunction) {
  return z.object({
    iqamaNumber: z
      .string()
      .min(1, t('common:validation.required'))
      .max(30, t('common:validation.maxLength', { max: 30 })),
    platformIdNumber: z
      .string()
      .min(1, t('common:validation.required'))
      .max(50, t('common:validation.maxLength', { max: 50 })),
    idExpiryDate: z.string().optional(),
    iqamaExpiryDate: z.string().optional(),
    drivingLicenseExpiryDate: z.string().optional(),
    insuranceExpiryDate: z.string().optional(),
  });
}

export type ProfileSubmissionFormValues = z.infer<ReturnType<typeof buildProfileSubmissionSchema>>;

// Mirrors AdminCreateEmployeeCommandValidator (Application/Employees/Commands/AdminCreateEmployee).
// Password complexity matches the self-registration rule (features/auth/schemas.ts) since both
// ultimately hit the same ASP.NET Identity password policy.
export function buildCreateEmployeeSchema(t: TFunction) {
  return z.object({
    fullName: z.string().min(1, t('common:validation.required')).max(200),
    email: z.string().min(1, t('common:validation.required')).email(t('admin.validation.emailInvalid')),
    password: z
      .string()
      .min(8, t('admin.validation.passwordMinRegister'))
      .regex(/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9])/, t('admin.validation.passwordComplexity')),
    phoneNumber: z.string().min(1, t('common:validation.required')),
    hasWhatsApp: z.boolean(),
    roles: z.array(z.string()).min(1, t('admin.validation.rolesRequired')),
    iqamaNumber: z.string().min(1, t('common:validation.required')).max(30),
    platformId: z.string().optional(),
    platformIdNumber: z.string().optional(),
    joiningDate: z.string().optional(),
    idExpiryDate: z.string().optional(),
    iqamaExpiryDate: z.string().optional(),
    drivingLicenseExpiryDate: z.string().optional(),
    insuranceExpiryDate: z.string().optional(),
  });
}

export type CreateEmployeeFormValues = z.infer<ReturnType<typeof buildCreateEmployeeSchema>>;

// Mirrors AdminUpdateEmployeeCommandValidator (Application/Employees/Commands/AdminUpdateEmployee).
export function buildEditEmployeeSchema(t: TFunction) {
  return z.object({
    fullName: z.string().min(1, t('common:validation.required')).max(200),
    iqamaNumber: z.string().optional(),
    platformId: z.string().optional(),
    idExpiryDate: z.string().optional(),
    iqamaExpiryDate: z.string().optional(),
    drivingLicenseExpiryDate: z.string().optional(),
    insuranceExpiryDate: z.string().optional(),
  });
}

export type EditEmployeeFormValues = z.infer<ReturnType<typeof buildEditEmployeeSchema>>;
